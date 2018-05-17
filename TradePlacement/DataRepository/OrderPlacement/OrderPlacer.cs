using TradePlacement.Api;
using TradePlacement.Domain.Exceptions;
using TradePlacement.Models;
using TradePlacement.Models.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradePlacement.TradePlacement
{
    public class OrderPlacer : IOrderPlacer
    {
        private readonly IApiClientFactory _apiClientFactory;

        public OrderPlacer(IApiClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        public async Task<PlaceExecutionReport> PlaceOrder(OrderWrapper orderWrapper)
        {
            var method = "SportsAPING/v1.0/placeOrders";

            var args = new Dictionary<string, object>
            {
                ["marketId"] = orderWrapper.MarketId
            };

            var limitOrder = new LimitOrder
            {
                PersistenceType = orderWrapper.Persistencetype,
                Price = orderWrapper.OrderTick.Price,
                Size = orderWrapper.OrderTick.Stake
            };

            var placeInstructions = new List<PlaceInstruction>()
            {
                new PlaceInstruction
                {
                    Handicap = 0,
                    Side = orderWrapper.Side,
                    OrderType = OrderType.LIMIT,
                    LimitOrder = limitOrder,
                    SelectionId = orderWrapper.SelectionId,
                }
            };

            args["instructions"] = placeInstructions;

            var apiClient = await _apiClientFactory.GetApiClient();
            var placeOrderReport = apiClient.GetData<JsonResponse<PlaceExecutionReport>>(method, args);

            if (placeOrderReport.Result.Status == ExecutionReportStatus.SUCCESS)
            {
                return placeOrderReport.Result;
            }

            if (placeOrderReport.Result.ErrorCode == ExecutionReportErrorCode.MARKET_SUSPENDED)
            {
                throw new MarketSuspendedException();
            }

            if (placeOrderReport.Result.ErrorCode == ExecutionReportErrorCode.BET_ACTION_ERROR)
            {
                throw new OrderActionErrorException();
            }

            if (placeOrderReport.Result.ErrorCode == ExecutionReportErrorCode.INSUFFICIENT_FUNDS)
            {
                throw new InsufficientFundsException();
            }

            throw new OrderNotPlaceableException();
        }

        public async Task<CancelExecutionReport> CancelOrder(string betId, string marketId)
        {
            var method = "SportsAPING/v1.0/cancelOrders";
            var args = new Dictionary<string, object>();
            var marketFilter = new MarketFilter();
            args["marketId"] = marketId;
            args["instructions"] = new List<CancelInstruction>() { new CancelInstruction() { BetId = betId } };

            var apiClient = await _apiClientFactory.GetApiClient();
            var cancelOrderReport = apiClient.GetData<JsonResponse<CancelExecutionReport>>(method, args);
            return cancelOrderReport.Result;
        }
    }
}