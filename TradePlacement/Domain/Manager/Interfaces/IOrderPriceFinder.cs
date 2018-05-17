using TradePlacement.Models;
using TradePlacement.Models.Api;

namespace TradePlacement.Domain.Manager
{
    public interface IOrderPriceFinder
    {
        OrderTick GetPrice(Side side, ExchangePrices prices);
    }
}