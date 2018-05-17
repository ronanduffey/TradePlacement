using Newtonsoft.Json;

namespace TradePlacement.Models.Api
{
    public class LoginDetails
    {
        [JsonProperty(PropertyName = "sessionToken")]
        public string SessionToken { get; set; }

        [JsonProperty(PropertyName = "loginStatus")]
        public string LoginStatus { get; set; }
    }
}