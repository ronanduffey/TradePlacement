using TradePlacement.Models.Raven;

namespace TradePlacement.DataRepository.Raven
{
    public interface ITradeStore
    {
        void AddTrade(Trade trade);
    }
}