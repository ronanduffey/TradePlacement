using TradePlacement.Domain.Manager;
using TradePlacement.Domain.StakeProviders.Opening;
using TradePlacement.Models.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace TradePlacementTests.Manager
{
    [TestClass]
    public class OrderPriceFinderTests
    {
        [TestMethod]
        public void ShouldThrowExceptionIfPricesAreNull()
        {
            var provider = new Mock<IOpeningStakeProvider>();
            provider.Setup(x => x.GetStake(It.IsAny<double>(), It.IsAny<List<double>>())).Returns(1);

            var mockStakeFactory = new Mock<IOpeningStakeProviderFactory>();
            mockStakeFactory.Setup(x => x.GetStakeProvider(It.IsAny<string>())).Returns(provider.Object);

            var prices = new ExchangePrices()
            {
                AvailableToBack = new List<PriceSize>()
                {
                    new PriceSize()
                    {
                        Price = 1,
                        Size = 2
                    }
                },
                AvailableToLay = new List<PriceSize>()
                {
                    new PriceSize()
                    {
                        Price = 5,
                        Size = 10
                    }
                }
            };

            var finder = new OrderPriceFinder(mockStakeFactory.Object);
            Assert.ThrowsException<System.Exception>(() => finder.GetPrice(TradePlacement.Models.Side.BACK, null));
        }
    }
}