using System;
using System.Threading.Tasks;
using TradePlacement.Models;

namespace TradePlacement.Domain.Manager
{
    public class TestClosingOrderManager : IClosingOrderManager
    {
        public Task ManageCloseout(OpenOrderResult openingOrderSummary)
        {
            throw new NotImplementedException();
        }
    }
}