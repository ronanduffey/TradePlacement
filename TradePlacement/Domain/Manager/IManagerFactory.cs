using System;
using TradePlacement.Models;

namespace TradePlacement.Domain.Manager
{
    public interface IManagerFactory
    {
        ManagerPair GetStrategyTradeManagerPair(Guid strategyId);
    }
}