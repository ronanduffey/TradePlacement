using TradePlacement.Models.Api;
using System.Threading.Tasks;

namespace TradePlacement.RunnerDataProvider
{
    public interface IRunnerService
    {
        Task<Runner> GetRunnerDetails(string marketId, long runnerId);
    }
}