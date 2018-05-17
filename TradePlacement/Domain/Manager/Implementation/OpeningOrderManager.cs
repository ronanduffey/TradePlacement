using TradePlacement.TradePlacement;
using TradePlacement.Models;
using TradePlacement.Models.Api;
using System;
using System.Linq;
using TradePlacement.RunnerDataProvider;
using System.Threading.Tasks;
using System.Collections.Generic;
using TradePlacement.SystemImplementation;
using Polly;
using TradePlacement.Domain.Exceptions;

namespace TradePlacement.Domain.Manager
{
    public class OpeningOrderManager : IOpeningOrderManager
    {
        private readonly IOrderPlacer _orderPlacer;
        private readonly IRunnerService _runnerService;
        private readonly IOrderPriceFinder _orderPriceFinder;
        private readonly ISleepService _sleepService;

        public OpeningOrderManager(IOrderPlacer orderPlacer, IRunnerService runnerService, ISleepService sleepService,
            IOrderPriceFinder orderPriceFinder)
        {
            _orderPlacer = orderPlacer;
            _runnerService = runnerService;
            _orderPriceFinder = orderPriceFinder;
            _sleepService = sleepService;
        }

        public async Task<OpenOrderResult> PlaceOpeningOrder(TradeDetail trade, IEnumerable<TradeDetail> relatedTrades)
        {
            var market = trade.Match.BetfairData.Markets.Single(x => x.MarketName == trade.MarketName);
            var runnerId = market.Runners.Single(x => x.Name == trade.RunnerName).Id;
            var latestRunnerBook = await _runnerService.GetRunnerDetails(market.MarketId, runnerId);

            var orderTick = _orderPriceFinder.GetPrice(trade.Side, latestRunnerBook.ExchangePrices);
            var openingOrderWrapper = new OrderWrapper(market.MarketId, runnerId, trade.Side, orderTick, PersistenceType.LAPSE);

            var policy = Policy.Handle<MarketSuspendedException>().Or<OrderActionErrorException>().Or<OrderNotPlaceableException>().WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt * 4)));

            var betReport = await policy.Execute(() => _orderPlacer.PlaceOrder(openingOrderWrapper));

            var betId = betReport.InstructionReports.Single().BetId;
            openingOrderWrapper.AddBetId(betId);
            _sleepService.Sleep(5000);

            var matchReport = await _runnerService.GetRunnerDetails(market.MarketId, runnerId);
            var orderReport = matchReport.Orders.Where(x => x.BetId == betId).Sum(x => x.SizeRemaining);

            if (orderReport != 0)
            {
                var cancelReport = await _orderPlacer.CancelOrder(betId, market.MarketId);
                if (cancelReport.Status == ExecutionReportStatus.SUCCESS)
                {
                    throw new OrderCancelledException(betId);
                }
            }

            return new OpenOrderResult(trade, openingOrderWrapper);
        }
    }
}