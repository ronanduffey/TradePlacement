using TradePlacement.Domain.StakeProviders.Opening;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TradePlacementTests.Domain.StakeProviders.Opening
{
    [TestClass]
    public class OpeningStakeProviderFactoryTests
    {
        public void ShouldReturnOpeningFixedStakeProviderWhenParameterIsFixed()
        {
            var factory = new OpeningStakeProviderFactory();
            var provider = factory.GetStakeProvider("Fixed");
            Assert.IsInstanceOfType(provider, typeof(OpeningFixedStakeProvider));
        }

        public void ShouldReturnOpeningDutchStakeProviderWhenParameterIsDutching()
        {
            var factory = new OpeningStakeProviderFactory();
            var provider = factory.GetStakeProvider("Dutching");
            Assert.IsInstanceOfType(provider, typeof(OpeningFixedStakeProvider));
        }

        public void ShouldThrowNotImplementedExceptionIfParameterNotMatched()
        {
            var factory = new OpeningStakeProviderFactory();
            Assert.ThrowsException<NotImplementedException>(() => factory.GetStakeProvider("N/A"));
        }
    }
}