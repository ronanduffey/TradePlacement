namespace TradePlacement.Domain.StakeProviders.Opening
{
    public interface IOpeningStakeProviderFactory
    {
        IOpeningStakeProvider GetStakeProvider(string name);
    }
}