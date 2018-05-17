namespace TradePlacement.Models
{
    public class OpenOrderResult
    {
        public TradeDetail OpenOrderRequest { get; }
        public OrderWrapper OpenOrderResponse { get; }

        public OpenOrderResult(TradeDetail openOrderRequest, OrderWrapper openOrderResponse)
        {
            OpenOrderRequest = openOrderRequest;
            OpenOrderResponse = openOrderResponse;
        }
    }
}