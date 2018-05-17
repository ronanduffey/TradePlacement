using System;

namespace TradePlacement.Domain.StakeProviders.Opening
{
    public class OpeningStakeProviderFactory : IOpeningStakeProviderFactory
    {
        public IOpeningStakeProvider GetStakeProvider(string stakeProviderType)
        {
            if (stakeProviderType == "Fixed")
            {
                return new OpeningFixedStakeProvider();
            }

            if (stakeProviderType == "Dutching")
            {
                return new OpeningDutchStakeProvider();
            }

            throw new NotImplementedException($"Stake provider {stakeProviderType} is not implemented!");
        }
    }
}