using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TradePlacement.Models.Api
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MatchProjection
    {
        NO_ROLLUP, ROLLED_UP_BY_PRICE, ROLLED_UP_BY_AVG_PRICE
    }
}