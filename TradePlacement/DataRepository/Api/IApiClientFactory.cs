using System.Threading.Tasks;

namespace TradePlacement.Api
{
    public interface IApiClientFactory
    {
        Task<IApiClient> GetApiClient();
    }
}