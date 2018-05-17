namespace TradePlacement.Domain.Exceptions
{
    public class InsufficientFundsException : OrderException
    {
        public InsufficientFundsException() : base("INSUFFICIENT_FUNDS", "Insufficient funds were available to place the order")
        {
        }
    }
}