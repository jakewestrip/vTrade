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
    public interface IDayPricesQuery
    {
        Task<DayPricesDto> Execute(string Ticker);
    }

    public class DayPricesQuery : IDayPricesQuery
    {
        private readonly IAmazonDynamoDB _amazonDynamoDB;

        public DayPricesQuery(IAmazonDynamoDB amazonDynamoDB)
        {
            _amazonDynamoDB = amazonDynamoDB;
        }

        public async Task<DayPricesDto> Execute(string Ticker)
        {
            try
            {
                var t = Ticker.ToUpper();

                var apiHitter = new HttpClient();
                apiHitter.BaseAddress = new Uri("https://www.asx.com.au/asx/1/share/");
                var json = await apiHitter.GetAsync(t + "/prices?count=365").Result.Content.ReadAsStringAsync();
                var u = JsonConvert.DeserializeObject<PricesResult>(json);

                return new DayPricesDto() {
                    Ticker = t,
                    PricePoints = u.Data.OrderBy(x => x.Close_Date).Select(x => new PricePoint
                    {
                        ClosePrice = x.Close_Price,
                        Date = x.Close_Date.ToShortDateString(),
                        Volume = x.Volume
                    }).ToList()
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class PricesResult
    {
        public List<PriceResultItem> Data { get; set; }
    }

    public class PriceResultItem
    {
        public string Code { get; set; }
        public DateTime Close_Date { get; set; }
        public double Close_Price { get; set; }
        public long Volume { get; set; }
    }
}
