using TradePlacement.Api;
using TradePlacement.TradePlacement;
using TradePlacement.Domain.Exceptions;
using TradePlacement.Models;
using TradePlacement.Models.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradePlacementTests.DataRepository.OrderPlacement
{
    [TestClass]
    public class OrderPlacerTests
    {
        private Mock<IApiClient> mockApiClient;

        [TestInitialize]
        public void Initialize()
        {
            mockApiClient = new Mock<IApiClient>();
        }

        [TestMethod]
        public async Task ShouldCallGetDataWithPlaceOrderMethod()
        {
            SetupApiClientResponse(new JsonResponse<PlaceExecutionReport>()
            {
                Result = new PlaceExecutionReport()
                {
                }
            });

            var mockApiClientFactory = SetupApiClientFactory();

            var orderPlacer = new OrderPlacer(mockApiClientFactory.Object);

            var orderWrapper = GetOrderWrapper();
            await orderPlacer.PlaceOrder(orderWrapper);
            mockApiClient.Verify(x => x.GetData<JsonResponse<PlaceExecutionReport>>("SportsAPING/v1.0/placeOrders", It.IsAny<Dictionary<string, object>>()));
        }

        [TestMethod]
        public async Task ShouldCallGetDataWithMarketId()
        {
            SetupApiClientResponse(new JsonResponse<PlaceExecutionReport>()
            {
                Result = new PlaceExecutionReport()
                {
                }
            });

            var mockApiClientFactory = SetupApiClientFactory();

            var orderPlacer = new OrderPlacer(mockApiClientFactory.Object);

            var orderWrapper = GetOrderWrapper();
            await orderPlacer.PlaceOrder(orderWrapper);
            mockApiClient.Verify(x => x.GetData<JsonResponse<PlaceExecutionReport>>(It.IsAny<string>(), It.Is<Dictionary<string, object>>(y => y.ContainsKey("marketId") && y["marketId"].ToString() == "A")));
        }

        [TestMethod]
        public async Task ShouldCallGetDataPlaceInstruction()
        {
            SetupApiClientResponse(new JsonResponse<PlaceExecutionReport>()
            {
                Result = new PlaceExecutionReport()
                {
                }
            });

            var mockApiClientFactory = SetupApiClientFactory();

            var orderPlacer = new OrderPlacer(mockApiClientFactory.Object);

            var orderWrapper = GetOrderWrapper();
            await orderPlacer.PlaceOrder(orderWrapper);
            mockApiClient.Verify(x => x.GetData<JsonResponse<PlaceExecutionReport>>(It.IsAny<string>(), It.Is<Dictionary<string, object>>(y => VerifyDetails(y, orderWrapper))));
        }

        private bool VerifyDetails(Dictionary<string, object> y, OrderWrapper orderWrapper)
        {
            var success = false;
            var instruction = y["instructions"] as List<PlaceInstruction>;
            success = 1 == instruction.Count;
            success = instruction.Single().Handicap == 0;
            success = instruction.Single().Side == orderWrapper.Side;
            success = instruction.Single().OrderType == OrderType.LIMIT;
            success = instruction.Single().LimitOrder.PersistenceType == orderWrapper.Persistencetype;
            success = instruction.Single().LimitOrder.Price == orderWrapper.OrderTick.Price;
            success = instruction.Single().LimitOrder.Size == orderWrapper.OrderTick.Stake;
            success = instruction.Single().SelectionId == orderWrapper.SelectionId;
            return success;
        }

        [TestMethod]
        public async Task ShouldThrowBetActionErrorWhenBetActionErrorResponse()
        {
            SetupApiClientResponse(new JsonResponse<PlaceExecutionReport>()
            {
                Result = new PlaceExecutionReport()
                {
                    Status = ExecutionReportStatus.FAILURE,
                    ErrorCode = ExecutionReportErrorCode.BET_ACTION_ERROR
                }
            });

            var mockApiClientFactory = SetupApiClientFactory();

            var orderPlacer = new OrderPlacer(mockApiClientFactory.Object);

            var orderWrapper = GetOrderWrapper();
            await Assert.ThrowsExceptionAsync<OrderActionErrorException>(() => orderPlacer.PlaceOrder(orderWrapper));
        }

        [TestMethod]
        public async Task ShouldThrowMarketSuspendedExceptionWhenMarketSuspendedResponse()
        {
            SetupApiClientResponse(new JsonResponse<PlaceExecutionReport>()
            {
                Result = new PlaceExecutionReport()
                {
                    Status = ExecutionReportStatus.FAILURE,
                    ErrorCode = ExecutionReportErrorCode.MARKET_SUSPENDED
                }
            });

            var mockApiClientFactory = SetupApiClientFactory();

            var orderPlacer = new OrderPlacer(mockApiClientFactory.Object);

            var orderWrapper = GetOrderWrapper();
            await Assert.ThrowsExceptionAsync<MarketSuspendedException>(() => orderPlacer.PlaceOrder(orderWrapper));
        }

        [TestMethod]
        public async Task ShouldThrowInsufficientFundsExceptionWhenInsufficientFundsResponse()
        {
            SetupApiClientResponse(new JsonResponse<PlaceExecutionReport>()
            {
                Result = new PlaceExecutionReport()
                {
                    Status = ExecutionReportStatus.FAILURE,
                    ErrorCode = ExecutionReportErrorCode.INSUFFICIENT_FUNDS
                }
            });

            var mockApiClientFactory = SetupApiClientFactory();

            var orderPlacer = new OrderPlacer(mockApiClientFactory.Object);

            var orderWrapper = GetOrderWrapper();
            await Assert.ThrowsExceptionAsync<InsufficientFundsException>(() => orderPlacer.PlaceOrder(orderWrapper));
        }

        [TestMethod]
        public async Task ShouldThrowOrderNotPlaceableExceptionWhenOrderNotPlaceableResponse()
        {
            SetupApiClientResponse(new JsonResponse<PlaceExecutionReport>()
            {
                Error = new TradePlacement.Models.Api.Exception(),
                Result = new PlaceExecutionReport()
                {
                    Status = ExecutionReportStatus.FAILURE,
                    ErrorCode = ExecutionReportErrorCode.LOSS_LIMIT_EXCEEDED
                }
            });

            var mockApiClientFactory = SetupApiClientFactory();
            var orderPlacer = new OrderPlacer(mockApiClientFactory.Object);

            var orderWrapper = GetOrderWrapper();
            await Assert.ThrowsExceptionAsync<OrderNotPlaceableException>(() => orderPlacer.PlaceOrder(orderWrapper));
        }

        private Mock<IApiClientFactory> SetupApiClientFactory()
        {
            var mockApiClientFactory = new Mock<IApiClientFactory>();
            mockApiClientFactory.Setup(x => x.GetApiClient()).Returns(Task.FromResult(mockApiClient.Object));
            return mockApiClientFactory;
        }

        private void SetupApiClientResponse(JsonResponse<PlaceExecutionReport> response)
        {
            mockApiClient.Setup(x => x.GetData<JsonResponse<PlaceExecutionReport>>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>())).Returns(response);
        }

        private OrderWrapper GetOrderWrapper()
        {
            return new OrderWrapper("A", 1, Side.BACK, new OrderTick(1, 2), PersistenceType.LAPSE);
        }
    }
}