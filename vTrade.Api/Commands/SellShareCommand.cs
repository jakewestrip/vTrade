using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vTrade.Models;

namespace vTrade.Queries
{
    public interface ISellShareCommand
    {
        Task<bool> Execute(string Ticker, int NumShares, int UserId);
    }

    public class SellShareCommand : ISellShareCommand
    {
        private readonly IAmazonDynamoDB _amazonDynamoDB;

        public SellShareCommand(IAmazonDynamoDB amazonDynamoDB)
        {
            _amazonDynamoDB = amazonDynamoDB;
        }

        public async Task<bool> Execute(string Ticker, int NumShares, int UserId)
        {
            try
            {
                var stockPriceQueryRequest = new QueryRequest
                {
                    TableName = "StockPrice",
                    ProjectionExpression = "Price",
                    KeyConditionExpression = "Ticker = :v_Ticker",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {":v_Ticker", new AttributeValue { S = Ticker }}
                    }
                };
                var stockPriceQuery = await _amazonDynamoDB.QueryAsync(stockPriceQueryRequest);
                var stockPrice = float.Parse(stockPriceQuery.Items.First().First().Value.N);

                var portfolioQueryRequest = new QueryRequest
                {
                    TableName = "Users",
                    ProjectionExpression = "OwnedStocks, Money",
                    KeyConditionExpression = "UserId = :v_Id",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {":v_Id", new AttributeValue { N = UserId.ToString() }}
                    }
                };
                var portfolioQuery = await _amazonDynamoDB.QueryAsync(portfolioQueryRequest);

                var portfolio = new UserPortfolio
                {
                    Money = 0,
                    UserId = UserId
                };

                if (portfolioQuery.Items.Any() && portfolioQuery.Items.First().Any(x => x.Key == "OwnedStocks") && portfolioQuery.Items.First().Any(x => x.Key == "Money"))
                {
                    portfolio.OwnedShares = portfolioQuery.Items.First().First(x => x.Key == "OwnedStocks").Value.M.Select(x => new OwnedShare { Ticker = x.Key, Shares = int.Parse(x.Value.N) }).ToList();
                    portfolio.Money = float.Parse(portfolioQuery.Items.First().First(x => x.Key == "Money").Value.N);
                }

                if(portfolio.OwnedShares.First(x => x.Ticker == Ticker).Shares > NumShares)
                {
                    portfolio.OwnedShares.First(x => x.Ticker == Ticker).Shares -= NumShares;
                }
                else
                {
                    portfolio.OwnedShares.Remove(portfolio.OwnedShares.First(x => x.Ticker == Ticker));
                }

                //GAIN MONEY
                portfolio.Money += (stockPrice * NumShares);

                //RESAVE
                var portfolioDict = portfolio.OwnedShares.ToDictionary(x => x.Ticker, x => new AttributeValue { N = x.Shares.ToString() });
                var portfolioMap = new AttributeValue { M = portfolioDict };
                var update = await _amazonDynamoDB.UpdateItemAsync(new UpdateItemRequest
                {
                    TableName = "Users",
                    Key = new Dictionary<string, AttributeValue> { { "UserId", new AttributeValue { N = UserId.ToString() } } },
                    AttributeUpdates = new Dictionary<string, AttributeValueUpdate>
                    {
                        { "Money", new AttributeValueUpdate { Action = AttributeAction.PUT, Value = new AttributeValue { N = portfolio.Money.ToString() } } },
                        { "OwnedStocks", new AttributeValueUpdate { Action = AttributeAction.PUT, Value = portfolioMap } }
                    }
                });

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
