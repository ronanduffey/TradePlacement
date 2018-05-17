using TradePlacement.Api;
using TradePlacement.Models.Api;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradePlacement.RunnerDataProvider
{
    public class RunnerService : IRunnerService
    {
        private readonly IApiClientFactory _apiClientFactory;

        public RunnerService(ApiClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        public async Task<Runner> GetRunnerDetails(string marketId, long runnerId)
        {
            var args = new Dictionary<string, object>
            {
                ["marketIds"] = new List<string>()
                {
                    marketId
                }
            };

            var priceData = new HashSet<PriceData>
            {
                PriceData.EX_TRADED,
                PriceData.EX_BEST_OFFERS
            };

            var p = new PriceProjection
            {
                PriceData = priceData
            };

            args["priceProjection"] = p;
            args["orderProjection"] = OrderProjection.ALL;
            args["matchProjection"] = MatchProjection.NO_ROLLUP;
            var method = "SportsAPING/v1.0/listMarketBook";
            var apiClient = await _apiClientFactory.GetApiClient();
            var jsonResponse = apiClient.GetData<JsonResponse<List<MarketBook>>>(method, args);
            var marketBooks = jsonResponse.Result;
            return marketBooks.Single().Runners.Single(x => x.SelectionId == runnerId);
        }
    }
}