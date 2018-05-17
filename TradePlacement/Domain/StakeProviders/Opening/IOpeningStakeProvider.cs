using System.Collections.Generic;

namespace TradePlacement.Domain.StakeProviders.Opening
{
    public interface IOpeningStakeProvider
    {
        double GetStake(double tradePrice, IEnumerable<double> relatedPrices);
    }
}