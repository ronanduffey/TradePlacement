using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TradePlacement.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Side
    {
        BACK, LAY, NONE
    }
}