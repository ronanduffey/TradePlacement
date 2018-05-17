using TradePlacement.Domain.StakeProviders.Opening;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TradePlacementTests.Domain.StakeProviders.Opening
{
    [TestClass]
    public class OpeningFixedStakeProviderTests
    {
        [TestMethod]
        public void ShouldReturn3ForStake()
        {
            var fixedStakeProvider = new OpeningFixedStakeProvider();
            var stake = fixedStakeProvider.GetStake(5, null);
            Assert.AreEqual(3, stake);
        }
    }
}