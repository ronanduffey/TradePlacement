namespace TradePlacement.Domain.Exceptions
{
    public class OrderActionErrorException : OrderException
    {
        public OrderActionErrorException() : base("BET_ACTION_ERROR", "Bet action error encountered when placing order")
        {
        }
    }
}