using System.Collections.Generic;
using TradePlacement.Models.Api;
using TradePlacement.Models;

namespace TradePlacement.Domain.StakeProviders.Closing
{
    public interface IClosingStakeCalculator
    {
        KeyValuePair<Side, double> GetFullHedgeStake(List<Order> runnerOrders, double currentPrice);
    }
}