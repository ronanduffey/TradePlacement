namespace TradePlacement.Domain.Exceptions
{
    public class OrderCancelledException : OrderException
    {
        public string BetId { get; }

        public OrderCancelledException(string betId) : base("CANCELLED", "Order was cancelled")
        {
            BetId = betId;
        }
    }
}