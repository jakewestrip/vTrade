using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vTrade.Models
{
    public class UserPortfolio
    {
        public int UserId { get; set; }

        public float Money { get; set; }
        public IList<OwnedShare> OwnedShares { get; set; }

    }

    public class OwnedShare
    {
        public string Ticker { get; set; }
        public int Shares { get; set; }
    }
}
