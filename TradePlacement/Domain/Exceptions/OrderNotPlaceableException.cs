namespace TradePlacement.Domain.Exceptions
{
    public class OrderNotPlaceableException : OrderException
    {
        public OrderNotPlaceableException() : base("ORDER_UNPLACEABLE", "An unhandled order exception was encountered")
        {
        }
    }
}