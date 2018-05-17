using TradePlacement.TradePlacement;
using TradePlacement.Domain.Manager;
using TradePlacement.Domain.StakeProviders.Closing;
using TradePlacement.Models;
using TradePlacement.Models.Api;
using TradePlacement.RunnerDataProvider;
using TradePlacement.SystemImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradePlacementTests.Manager
{
    [TestClass]
    public class ClosingOrderManagerTests
    {
        private ClosingOrderManager manager;
        private Mock<ISleepService> mockSleepService;
        private Mock<IRunnerService> mockRunnerService;
        private Mock<IOrderPlacer> mockOrderPlacer;
        private Mock<IClosingStakeCalculator> mockClosingStakeCalculator;
        private TradeDetail tradeDetail;
        private OrderWrapper orderWrapper;

        [TestInitialize]
        public void Initialize()
        {
            mockSleepService = new Mock<ISleepService>();
            mockSleepService.Setup(x => x.Sleep(It.IsAny<int>()));

            mockRunnerService = new Mock<IRunnerService>();
            mockRunnerService.Setup(x => x.GetRunnerDetails(It.IsAny<string>(), It.IsAny<long>())).Returns(Task.FromResult(new TradePlacement.Models.Api.Runner()
            {
                ExchangePrices = new ExchangePrices()
                {
                    AvailableToBack = new List<PriceSize>()
                    {
                        new PriceSize()
                        {
                            Price = 5,
                            Size = 10
                        }
                    },
                    AvailableToLay = new List<PriceSize>()
                    {
                        new PriceSize()
                        {
                            Price  = 8,
                            Size = 15
                        }
                    }
                },
                Orders = new List<Order>()
                {
                    new Order()
                    {
                        BetId = "ABC",
                    },
                    new Order()
                    {
                        BetId = "XYZ",
                        SizeRemaining = 0
                    }
                }
            }));

            mockOrderPlacer = new Mock<IOrderPlacer>();
            mockOrderPlacer.Setup(x => x.PlaceOrder(It.IsAny<OrderWrapper>())).Returns(Task.FromResult(new PlaceExecutionReport()
            {
                InstructionReports = new List<PlaceInstructionReport>()
                {
                    new PlaceInstructionReport()
                    {
                        BetId = "XYZ"
                    }
                }
            }));

            mockClosingStakeCalculator = new Mock<IClosingStakeCalculator>();
            mockClosingStakeCalculator.Setup(x => x.GetFullHedgeStake(It.IsAny<List<Order>>(), It.IsAny<double>())).Returns(new KeyValuePair<Side, double>(Side.BACK, 200));

            tradeDetail = new TradeDetail()
            {
                Tracking = 100
            };

            orderWrapper = new OrderWrapper("A", 1, Side.BACK, new OrderTick(2, 3), PersistenceType.LAPSE);
            orderWrapper.AddBetId("ABC");
        }

        [TestMethod]
        public async Task ShouldCallSleepServiceWithTrackingValue()
        {
            manager = new ClosingOrderManager(mockSleepService.Object, mockRunnerService.Object, mockOrderPlacer.Object, mockClosingStakeCalculator.Object);
            await manager.ManageCloseout(new OpenOrderResult(tradeDetail, orderWrapper));
            mockSleepService.Verify(x => x.Sleep(100), Times.Once);
        }

        [TestMethod]
        public async Task ShouldCallGetRunnerDetailsWithMarketId()
        {
            manager = new ClosingOrderManager(mockSleepService.Object, mockRunnerService.Object, mockOrderPlacer.Object, mockClosingStakeCalculator.Object);
            await manager.ManageCloseout(new OpenOrderResult(tradeDetail, orderWrapper));
            mockRunnerService.Verify(x => x.GetRunnerDetails("A", 1), Times.Exactly(2));
        }

        [TestMethod]
        public async Task ShouldCallGetFullHedgeStakeWithOpeningOrders()
        {
            manager = new ClosingOrderManager(mockSleepService.Object, mockRunnerService.Object, mockOrderPlacer.Object, mockClosingStakeCalculator.Object);
            await manager.ManageCloseout(new OpenOrderResult(tradeDetail, orderWrapper));
            mockClosingStakeCalculator.Verify(x => x.GetFullHedgeStake(It.Is<List<Order>>(y => y.Count() == 1), It.IsAny<double>()), Times.Once);
        }

        [TestMethod]
        public async Task ShouldCallGetFullHedgeStakeWithClosingPrice()
        {
            manager = new ClosingOrderManager(mockSleepService.Object, mockRunnerService.Object, mockOrderPlacer.Object, mockClosingStakeCalculator.Object);
            await manager.ManageCloseout(new OpenOrderResult(tradeDetail, orderWrapper));
            mockClosingStakeCalculator.Verify(x => x.GetFullHedgeStake(It.IsAny<List<Order>>(), 8), Times.Once);
        }

        [TestMethod]
        public async Task ShouldCallPlaceOrderWithMarketId()
        {
            manager = new ClosingOrderManager(mockSleepService.Object, mockRunnerService.Object, mockOrderPlacer.Object, mockClosingStakeCalculator.Object);
            await manager.ManageCloseout(new OpenOrderResult(tradeDetail, orderWrapper));
            mockOrderPlacer.Verify(x => x.PlaceOrder(It.Is<OrderWrapper>(y => y.MarketId == "A")), Times.Once);
        }

        [TestMethod]
        public async Task ShouldCallPlaceOrderWithSelectionId()
        {
            manager = new ClosingOrderManager(mockSleepService.Object, mockRunnerService.Object, mockOrderPlacer.Object, mockClosingStakeCalculator.Object);
            await manager.ManageCloseout(new OpenOrderResult(tradeDetail, orderWrapper));
            mockOrderPlacer.Verify(x => x.PlaceOrder(It.Is<OrderWrapper>(y => y.SelectionId == 1)), Times.Once);
        }

        [TestMethod]
        public async Task ShouldCallPlaceOrderWithSide()
        {
            manager = new ClosingOrderManager(mockSleepService.Object, mockRunnerService.Object, mockOrderPlacer.Object, mockClosingStakeCalculator.Object);
            await manager.ManageCloseout(new OpenOrderResult(tradeDetail, orderWrapper));
            mockOrderPlacer.Verify(x => x.PlaceOrder(It.Is<OrderWrapper>(y => y.Side == Side.BACK)), Times.Once);
        }

        [TestMethod]
        public async Task ShouldCallPlaceOrderWithPrice()
        {
            manager = new ClosingOrderManager(mockSleepService.Object, mockRunnerService.Object, mockOrderPlacer.Object, mockClosingStakeCalculator.Object);
            await manager.ManageCloseout(new OpenOrderResult(tradeDetail, orderWrapper));
            mockOrderPlacer.Verify(x => x.PlaceOrder(It.Is<OrderWrapper>(y => y.OrderTick.Price == 8)), Times.Once);
        }

        [TestMethod]
        public async Task ShouldCallPlaceOrderWithStake()
        {
            manager = new ClosingOrderManager(mockSleepService.Object, mockRunnerService.Object, mockOrderPlacer.Object, mockClosingStakeCalculator.Object);
            await manager.ManageCloseout(new OpenOrderResult(tradeDetail, orderWrapper));
            mockOrderPlacer.Verify(x => x.PlaceOrder(It.Is<OrderWrapper>(y => y.OrderTick.Stake == 200)), Times.Once);
        }

        [TestMethod]
        public async Task ShouldCallPlaceOrderWithPersistenceType()
        {
            manager = new ClosingOrderManager(mockSleepService.Object, mockRunnerService.Object, mockOrderPlacer.Object, mockClosingStakeCalculator.Object);
            await manager.ManageCloseout(new OpenOrderResult(tradeDetail, orderWrapper));
            mockOrderPlacer.Verify(x => x.PlaceOrder(It.Is<OrderWrapper>(y => y.Persistencetype == PersistenceType.LAPSE)), Times.Once);
        }

        [TestMethod]
        public async Task ShouldCallSleepServiceWith10SecondWait()
        {
            manager = new ClosingOrderManager(mockSleepService.Object, mockRunnerService.Object, mockOrderPlacer.Object, mockClosingStakeCalculator.Object);
            await manager.ManageCloseout(new OpenOrderResult(tradeDetail, orderWrapper));
            mockSleepService.Verify(x => x.Sleep(10000), Times.Once);
        }

        [TestMethod]
        public async Task ShouldThrowExceptionIfOrderReportIsNotSuccess()
        {
            mockOrderPlacer.Setup(x => x.PlaceOrder(It.IsAny<OrderWrapper>())).Returns(Task.FromResult(new PlaceExecutionReport()
            {
                Status = ExecutionReportStatus.FAILURE
            }));

            manager = new ClosingOrderManager(mockSleepService.Object, mockRunnerService.Object, mockOrderPlacer.Object, mockClosingStakeCalculator.Object);
            await Assert.ThrowsExceptionAsync<System.Exception>(() => manager.ManageCloseout(new OpenOrderResult(tradeDetail, orderWrapper)));
        }
    }
}