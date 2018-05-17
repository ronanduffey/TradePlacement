using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace TradePlacement.Models.Raven
{
    public class Trade
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

        public string OpeningId { get; set; }
        public List<string> ClosingIds { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}