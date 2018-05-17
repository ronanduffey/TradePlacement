using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TradePlacement.Models.Api
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderProjection
    {
        ALL, EXECUTABLE, EXECUTION_COMPLETE
    }
}