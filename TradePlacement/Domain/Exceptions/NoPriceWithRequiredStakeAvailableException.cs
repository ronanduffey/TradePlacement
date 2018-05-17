namespace TradePlacement.Domain.Exceptions
{
    public class NoPriceWithRequiredStakeAvailableException : OrderException
    {
        public NoPriceWithRequiredStakeAvailableException() : base("NO_PRICES_AVAILABLE", "No prices available for stake")
        {
        }
    }
}