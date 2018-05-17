using TradePlacement.Models;
using TradePlacement.Models.Api;
using System.Threading.Tasks;

namespace TradePlacement.TradePlacement
{
    public interface IOrderPlacer
    {
        Task<PlaceExecutionReport> PlaceOrder(OrderWrapper orderWrapper);

        Task<CancelExecutionReport> CancelOrder(string betId, string marketId);
    }
}