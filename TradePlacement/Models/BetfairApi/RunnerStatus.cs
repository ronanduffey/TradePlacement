using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TradePlacement.Models.Api
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RunnerStatus
    {
        ACTIVE, WINNER, LOSER, REMOVED_VACANT, REMOVED
    }
}