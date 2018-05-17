using TradePlacement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradePlacement.Domain.Manager
{
    public interface IOpeningOrderManager
    {
        Task<OpenOrderResult> PlaceOpeningOrder(TradeDetail trade, IEnumerable<TradeDetail> relatedTrades);
    }
}