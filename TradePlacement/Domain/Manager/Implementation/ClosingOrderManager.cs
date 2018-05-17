using TradePlacement.TradePlacement;
using TradePlacement.Domain.StakeProviders.Closing;
using TradePlacement.Models;
using TradePlacement.Models.Api;
using TradePlacement.RunnerDataProvider;
using TradePlacement.SystemImplementation;
using System.Linq;
using System.Threading.Tasks;

namespace TradePlacement.Domain.Manager
{
    public class ClosingOrderManager : IClosingOrderManager
    {
        private readonly ISleepService _sleepService;
        private readonly IRunnerService _runnerService;
        private readonly IOrderPlacer _orderPlacer;
        private readonly IClosingStakeCalculator _closingStakeCalculator;

        public ClosingOrderManager(ISleepService sleepService, IRunnerService runnerService, IOrderPlacer orderPlacer,
            IClosingStakeCalculator closingStakeCalculator)
        {
            _sleepService = sleepService;
            _runnerService = runnerService;
            _orderPlacer = orderPlacer;
            _closingStakeCalculator = closingStakeCalculator;
        }

        public async Task ManageCloseout(OpenOrderResult openingOrderSummary)
        {
            if (openingOrderSummary.OpenOrderRequest.Tracking.HasValue)
            {
                _sleepService.Sleep(openingOrderSummary.OpenOrderRequest.Tracking.Value);
                await CloseoutBet(openingOrderSummary);
            }
            else
            {
                return;
            }
        }

        private async Task CloseoutBet(OpenOrderResult openingOrderSummary)
        {
            var marketId = openingOrderSummary.OpenOrderResponse.MarketId;
            var runnerId = openingOrderSummary.OpenOrderResponse.SelectionId;
            var betId = openingOrderSummary.OpenOrderResponse.BetId;

            var latestRunnerBook = await _runnerService.GetRunnerDetails(marketId, runnerId);
            var openingOrders = latestRunnerBook.Orders.Where(x => x.BetId == betId).ToList();

            double closingPrice = 0;
            if (openingOrderSummary.OpenOrderRequest.TrackingTradeOffset.HasValue)
            {
                closingPrice = PriceLadderUtility.GetWinningPriceTarget(Side.BACK, openingOrderSummary.OpenOrderResponse.OrderTick.Price, openingOrderSummary.OpenOrderRequest.TrackingTradeOffset.Value);
            }
            else if (openingOrderSummary.OpenOrderResponse.Side == Side.BACK)
            {
                closingPrice = latestRunnerBook.ExchangePrices.AvailableToLay.Min(x => x.Price);
            }
            else if (openingOrderSummary.OpenOrderResponse.Side == Side.LAY)
            {
                closingPrice = latestRunnerBook.ExchangePrices.AvailableToBack.Max(x => x.Price);
            }
            else
            {
                throw new System.Exception("No closing side specified");
            }

            var closeoutStake = _closingStakeCalculator.GetFullHedgeStake(openingOrders, closingPrice);

            var orderWrapper = new OrderWrapper(marketId, runnerId, closeoutStake.Key, new OrderTick(closingPrice, closeoutStake.Value), PersistenceType.LAPSE);
            var closeoutOrderReport = await _orderPlacer.PlaceOrder(orderWrapper);

            if (closeoutOrderReport.Status != ExecutionReportStatus.SUCCESS)
            {
                throw new System.Exception("Closeout bet not placed!");
            }

            var closeoutBetId = closeoutOrderReport.InstructionReports.Single().BetId;
            _sleepService.Sleep(10000);
            await ManageCloseoutOrderMatching(marketId, runnerId, closeoutBetId);
        }

        private async Task ManageCloseoutOrderMatching(string marketId, long runnerId, string closeoutBetId)
        {
            var latestRunnerBook = await _runnerService.GetRunnerDetails(marketId, runnerId);
            var runnerOrders = latestRunnerBook.Orders.Where(x => x.BetId == closeoutBetId);
            if (runnerOrders.Sum(x => x.SizeRemaining) != 0)
            {
                throw new System.Exception("Closing bet was not fully matched after ten seconds!");
            }
        }
    }
}