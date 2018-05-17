using TradePlacement.Domain.Manager;

namespace TradePlacement.Models
{
    public class ManagerPair
    {
        public IOpeningOrderManager OpeningOrderManager { get; }
        public IClosingOrderManager ClosingOrderManager { get; }

        public ManagerPair(IOpeningOrderManager openingOrderManager, IClosingOrderManager closingOrderManager)
        {
            OpeningOrderManager = openingOrderManager;
            ClosingOrderManager = closingOrderManager;
        }
    }
}