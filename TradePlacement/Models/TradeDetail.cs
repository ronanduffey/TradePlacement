using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Converters;

namespace TradePlacement.Models
{
    public class TradeDetail
    {
        public Guid Id { get; set; }
        public Guid StrategyId { get; set; }
        public Match Match { get; set; }
        public string MarketName { get; set; }
        public string RunnerName { get; set; }
        public int? Tracking { get; set; }
        public int? TrackingTradeOffset { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Side Side { get; set; }
    }
}