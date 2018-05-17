using System;
using System.Collections.Generic;
using System.Linq;

namespace TradePlacement.Domain.StakeProviders.Opening
{
    public class OpeningDutchStakeProvider : IOpeningStakeProvider
    {
        public double GetStake(double tradePrice, IEnumerable<double> relatedPrices)
        {
            if (relatedPrices == null)
            {
                throw new Exception("A list of related book prices must be provided for an accurate dutch stake");
            }

            if (tradePrice == 0 || relatedPrices.Any(x => x == 0))
            {
                throw new Exception("A price of 0 is not valid");
            }

            var bookCover = (1 / tradePrice) + (relatedPrices.Select(x => 1 / x).Sum());
            var runnerCover = (1 / tradePrice) / bookCover;
            return runnerCover * 30;
        }
    }
}