using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace TradePlacement.Models.Api
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PriceData
    {
        SP_AVAILABLE, SP_TRADED,
        EX_BEST_OFFERS, EX_ALL_OFFERS, EX_TRADED,
    }
}