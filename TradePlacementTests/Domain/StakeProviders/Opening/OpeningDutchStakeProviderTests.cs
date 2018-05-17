using TradePlacement.Domain.StakeProviders.Opening;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TradePlacementTests.Domain.StakeProviders.Opening
{
    [TestClass]
    public class OpeningDutchStakeProviderTests
    {
        [TestMethod]
        public void ShouldReturnCorrectDutchStake()
        {
            var tradePrice = 3.25;
            var otherPrices = new List<double>()
            {
                4.6,
                10
            };

            var stakeProvider = new OpeningDutchStakeProvider();
            Assert.AreEqual(14.77, stakeProvider.GetStake(tradePrice, otherPrices), 0.05);
        }

        [TestMethod]
        public void ShouldThrowExceptionIfRelatedPricesAreNull()
        {
            var tradePrice = 3.25;
            var stakeProvider = new OpeningDutchStakeProvider();
            Assert.ThrowsException<Exception>(() => stakeProvider.GetStake(tradePrice, null));
        }

        [TestMethod]
        public void ShouldThrowExceptionIfTradePriceIs0()
        {
            var tradePrice = 0;
            var otherPrices = new List<double>()
            {
                4.6,
                10
            };
            var stakeProvider = new OpeningDutchStakeProvider();
            Assert.ThrowsException<Exception>(() => stakeProvider.GetStake(tradePrice, otherPrices));
        }

        [TestMethod]
        public void ShouldThrowExceptionIfRelatedPricesAre0()
        {
            var tradePrice = 3.25;
            var otherPrices = new List<double>()
            {
                4.6,
                0
            };
            var stakeProvider = new OpeningDutchStakeProvider();
            Assert.ThrowsException<Exception>(() => stakeProvider.GetStake(tradePrice, otherPrices));
        }
    }
}