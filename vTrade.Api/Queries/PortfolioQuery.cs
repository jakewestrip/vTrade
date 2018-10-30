using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vTrade.Models;

namespace vTrade.Queries
{
    public interface IPortfolioQuery
    {
        Task<PortfolioDto> Execute(int UserId);
    }

    public class PortfolioQuery : IPortfolioQuery
    {
        private readonly IAmazonDynamoDB _amazonDynamoDB;

        public PortfolioQuery(IAmazonDynamoDB amazonDynamoDB)
        {
            _amazonDynamoDB = amazonDynamoDB;
        }

        public async Task<PortfolioDto> Execute(int UserId)
        {
            try
            {
                var stockPriceScan = await _amazonDynamoDB.ScanAsync("StockPrice", new List<string>() { "Ticker", "Price" });
                var stockPriceResult = stockPriceScan.Items.Select(x => new PortfolioDtoRow
                {
                    Ticker = x["Ticker"].S,
                    Price = float.Parse(x["Price"].N)
                }).ToList();

                var portfolioQueryRequest = new QueryRequest
                {
                    TableName = "Users",
                    ProjectionExpression = "OwnedStocks, Money",
                    KeyConditionExpression = "UserId = :v_Id",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":v_Id", new AttributeValue { N = UserId.ToString() }}
                }};
                var portfolioQuery = await _amazonDynamoDB.QueryAsync(portfolioQueryRequest);
                var money = 0f;
                if (portfolioQuery.Items.Any() && portfolioQuery.Items.First().Any(x => x.Key == "OwnedStocks") && portfolioQuery.Items.First().Any(x => x.Key == "Money"))
                {
                    money = float.Parse(portfolioQuery.Items.First().First(x => x.Key == "Money").Value.N);

                    var portfolio = portfolioQuery.Items.First().First(x => x.Key == "OwnedStocks").Value.M.Select(x => new { Ticker = x.Key, Shares = x.Value.N }).ToList();

                    portfolio.ForEach(x =>
                    {
                        stockPriceResult.First(y => y.Ticker == x.Ticker).OwnedShares = int.Parse(x.Shares);
                    });
                }

                return new PortfolioDto() { Rows = stockPriceResult, Money = money };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
