using TradePlacement.DataRepository.Raven;
using TradePlacement.Domain.Manager;
using TradePlacement.MessageProcessor;
using TradePlacement.Models;
using TradePlacement.SystemImplementation;
using TradePlacement.SystemImplementation.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradePlacementTests.MessageProcessor
{
    [TestClass]
    public class TradeMessageProcessorTests
    {
        [TestMethod]
        public async Task ShouldCallCopyFileWithMetadataLocation()
        {
            var mockOpenOrderManager = new Mock<IOpeningOrderManager>();
            mockOpenOrderManager.Setup(x => x.PlaceOpeningOrder(It.IsAny<TradeDetail>(), It.IsAny<IEnumerable<TradeDetail>>())).Returns(Task.FromResult(new OpenOrderResult(null, null)));

            var mockManagerFactory = new Mock<IManagerFactory>();

            mockManagerFactory.Setup(x => x.GetStrategyTradeManagerPair(It.IsAny<Guid>())).Returns(new ManagerPair(mockOpenOrderManager.Object, null));

            var mockTradeStore = new Mock<ITradeStore>();

            var mockConsole = new Mock<IConsole>();

            var mockFile = new Mock<IFile>();

            var tradeMessageProcessor = new TradeMessageProcessor(mockManagerFactory.Object, mockTradeStore.Object, mockConsole.Object, mockFile.Object);

            await tradeMessageProcessor.ProcessMessage(new TradeDetail()
            {
                Match = new TradePlacement.Models.Match()
                {
                    WhoScoredData = new MatchDetail()
                    {
                        MatchMetadataLocation = "ABC"
                    }
                }
            });

            mockFile.Verify(x => x.Copy(It.Is<string>(y => y == "ABC"), It.IsAny<string>()), Times.Once);
        }
    }
}