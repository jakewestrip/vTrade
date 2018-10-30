using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vTrade.Models
{
    public class PortfolioDto
    {
        public float Money { get; set; }
        public IList<PortfolioDtoRow> Rows { get; set; }
    }

    public class PortfolioDtoRow
    {
        public string Ticker { get; set; }
        public float Price { get; set; }
        public int OwnedShares { get; set; }
    }
}
