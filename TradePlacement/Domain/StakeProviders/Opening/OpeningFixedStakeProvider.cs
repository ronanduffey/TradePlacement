using System.Collections.Generic;

namespace TradePlacement.Domain.StakeProviders.Opening
{
    public class OpeningFixedStakeProvider : IOpeningStakeProvider
    {
        public double GetStake(double tradePrice, IEnumerable<double> relatedPrices)
        {
            return 3;
        }
    }
}