namespace TradePlacement.Domain.Exceptions
{
    public class MarketSuspendedException : OrderException
    {
        public MarketSuspendedException() : base("MARKET_SUSPENDED", "The market was suspended at order placement time")
        {
        }
    }
}