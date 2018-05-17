using TradePlacement.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TradePlacement.Domain.Manager
{
    public class TestOpeningOrderManager : IOpeningOrderManager
    {
        public Task<OpenOrderResult> PlaceOpeningOrder(TradeDetail trade, IEnumerable<TradeDetail> relatedTrades)
        {
            throw new NotImplementedException();
        }
    }
}