using System.Collections.Generic;

namespace TradePlacement.Models
{
    public class Market
    {
        public string MarketName { get; set; }
        public string MarketId { get; set; }
        public List<Runner> Runners { get; set; }
    }
}