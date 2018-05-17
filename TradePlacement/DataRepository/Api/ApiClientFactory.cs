using TradePlacement.Models;
using System.Threading.Tasks;

namespace TradePlacement.Api
{
    public class ApiClientFactory : IApiClientFactory
    {
        public async Task<IApiClient> GetApiClient()
        {
            var credentials = Credentials.Instance;
            var betfairLogin = new BetfairLogin();
            await betfairLogin.Login(credentials.Username, credentials.Password, credentials.AppKey);
            return new ApiClient(betfairLogin);
        }
    }
}