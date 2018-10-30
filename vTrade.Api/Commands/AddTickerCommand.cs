using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using vTrade.Models;

namespace vTrade.Queries
{
    public interface IAddTickerCommand
    {
        Task<bool> Execute(string Ticker);
    }

    public class AddTickerCommand : IAddTickerCommand
    {
        private readonly IAmazonDynamoDB _amazonDynamoDB;

        public AddTickerCommand(IAmazonDynamoDB amazonDynamoDB)
        {
            _amazonDynamoDB = amazonDynamoDB;
        }

        public async Task<bool> Execute(string Ticker)
        {
            try
            {
                var t = Ticker.ToUpper();
                var stockPriceScan = await _amazonDynamoDB.ScanAsync("StockPrice", new List<string>() { "Ticker" });
                var stockPriceResult = stockPriceScan.Items.Select(x => x["Ticker"].S).ToList();

                if (stockPriceResult.Contains(t))
                    return false;

                var apiHitter = new HttpClient();
                apiHitter.BaseAddress = new Uri("https://www.asx.com.au/asx/1/share/");
                var json = apiHitter.GetAsync(t).Result.Content.ReadAsStringAsync().Result;
                var u = JsonConvert.DeserializeObject<PriceUpdate>(json);
                var price = u.Last_Price;

                var put = await _amazonDynamoDB.PutItemAsync(new PutItemRequest
                {
                    TableName = "StockPrice",
                    Item = new Dictionary<string, AttributeValue> {
                        { "Ticker", new AttributeValue { S = t } },
                        { "Price", new AttributeValue { N = price.ToString() } }
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

    public class PriceUpdate
    {
        public string Code { get; set; }
        public string Last_Price { get; set; }
    }
}
