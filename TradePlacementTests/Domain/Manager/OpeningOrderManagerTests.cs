using TradePlacement.TradePlacement;
using TradePlacement.Domain.Manager;
using TradePlacement.Models;
using TradePlacement.Models.Api;
using TradePlacement.RunnerDataProvider;
using TradePlacement.SystemImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradePlacementTests.Manager
{
    [TestClass]
    public class OpeningOrderManagerTests
    {
        [TestMethod]
        public async Task ShouldCallGetPriceWithCorrectParameters()
        {
            var orderPlacer = new Mock<IOrderPlacer>();
            orderPlacer.Setup(x => x.PlaceOrder(It.IsAny<OrderWrapper>())).Returns(Task.FromResult(new PlaceExecutionReport()
            {
                InstructionReports = new List<PlaceInstructionReport>()
                {
                    new PlaceInstructionReport()
                    {
                        BetId = "BetId"
                    }
                }
            }));

            var runnerService = new Mock<IRunnerService>();
            var runner = new TradePlacement.Models.Api.Runner()
            {
                ExchangePrices = new ExchangePrices()
                {
                    AvailableToBack = new List<PriceSize>()
                    {
                        new PriceSize()
                        {
                            Price = 10,
                            Size = 20
                        }
                    }
                },
                Orders = new List<Order>()
                {
                    new Order()
                    {
                        BetId = "BetId"
                    }
                }
            };
            runnerService.Setup(x => x.GetRunnerDetails(It.IsAny<string>(), It.IsAny<long>())).Returns(Task.FromResult(runner));

            var orderPriceFinder = new Mock<IOrderPriceFinder>();
            orderPriceFinder.Setup(x => x.GetPrice(It.IsAny<Side>(), It.IsAny<ExchangePrices>())).Returns(new OrderTick(1, 2));

            var sleepService = new Mock<ISleepService>();
            sleepService.Setup(x => x.Sleep(It.IsAny<int>()));

            var manager = new OpeningOrderManager(orderPlacer.Object, runnerService.Object, sleepService.Object, orderPriceFinder.Object);

            var tradeDetail = new TradeDetail()
            {
                Match = new TradePlacement.Models.Match()
                {
                    BetfairData = new BetfairEvent()
                    {
                        Markets = new List<Market>()
                        {
                            new Market()
                            {
                                MarketId = "A",
                                MarketName = "Name",
                                Runners = new  List<TradePlacement.Models.Runner>()
                                {
                                    new TradePlacement.Models.Runner()
                                    {
                                        Id = 1,
                                        Name = "Runner"
                                    }
                                }
                            }
                        }
                    }
                },
                MarketName = "Name",
                RunnerName = "Runner"
            };

            await manager.PlaceOpeningOrder(tradeDetail, null);
            orderPriceFinder.Verify(x => x.GetPrice(tradeDetail.Side, runner.ExchangePrices));
        }

        [TestMethod]
        public async Task ShouldCallPlaceOrderWithCorrectParameters()
        {
            var orderPlacer = new Mock<IOrderPlacer>();
            orderPlacer.Setup(x => x.PlaceOrder(It.IsAny<OrderWrapper>())).Returns(Task.FromResult(new PlaceExecutionReport()
            {
                InstructionReports = new List<PlaceInstructionReport>()
                {
                    new PlaceInstructionReport()
                    {
                        BetId = "BetId"
                    }
                }
            }));

            var runnerService = new Mock<IRunnerService>();
            var runner = new TradePlacement.Models.Api.Runner()
            {
                ExchangePrices = new ExchangePrices()
                {
                    AvailableToBack = new List<PriceSize>()
                    {
                        new PriceSize()
                        {
                            Price = 10,
                            Size = 20
                        }
                    }
                },
                Orders = new List<Order>()
                {
                    new Order()
                    {
                        BetId = "BetId"
                    }
                },
                SelectionId = 1
            };
            runnerService.Setup(x => x.GetRunnerDetails(It.IsAny<string>(), It.IsAny<long>())).Returns(Task.FromResult(runner));

            var orderPriceFinder = new Mock<IOrderPriceFinder>();
            orderPriceFinder.Setup(x => x.GetPrice(It.IsAny<Side>(), It.IsAny<ExchangePrices>())).Returns(new OrderTick(1, 2));

            var sleepService = new Mock<ISleepService>();
            sleepService.Setup(x => x.Sleep(It.IsAny<int>()));

            var manager = new OpeningOrderManager(orderPlacer.Object, runnerService.Object, sleepService.Object, orderPriceFinder.Object);

            var tradeDetail = new TradeDetail()
            {
                Match = new TradePlacement.Models.Match()
                {
                    BetfairData = new BetfairEvent()
                    {
                        Markets = new List<Market>()
                        {
                            new Market()
                            {
                                MarketId = "A",
                                MarketName = "Name",
                                Runners = new  List<TradePlacement.Models.Runner>()
                                {
                                    new TradePlacement.Models.Runner()
                                    {
                                        Id = 1,
                                        Name = "Runner"
                                    }
                                }
                            }
                        }
                    }
                },
                MarketName = "Name",
                RunnerName = "Runner"
            };

            await manager.PlaceOpeningOrder(tradeDetail, null);
            orderPlacer.Verify(x => x.PlaceOrder(It.Is<OrderWrapper>(y => y.MarketId == "A")));
            orderPlacer.Verify(x => x.PlaceOrder(It.Is<OrderWrapper>(y => y.SelectionId == runner.SelectionId)));
            orderPlacer.Verify(x => x.PlaceOrder(It.Is<OrderWrapper>(y => y.Side == tradeDetail.Side)));
            orderPlacer.Verify(x => x.PlaceOrder(It.Is<OrderWrapper>(y => y.OrderTick.Price == 1)));
        }
    }
}