using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vTrade.Models
{
    public class DayPricesDto
    {
        public string Ticker { get; set; }
        public List<PricePoint> PricePoints { get; set; }
    }

    public class PricePoint
    {
        public string Date { get; set; }
        public double ClosePrice { get; set; }
        public long Volume { get; set; }
    }
}
