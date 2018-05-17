using TradePlacement.Api;
using TradePlacement.TradePlacement;
using TradePlacement.Domain.StakeProviders.Closing;
using TradePlacement.Domain.StakeProviders.Opening;
using TradePlacement.Models;
using TradePlacement.RunnerDataProvider;
using TradePlacement.SystemImplementation;
using System;
using System.Collections.Generic;

namespace TradePlacement.Domain.Manager
{
    public class ManagerFactory : IManagerFactory
    {
        private readonly object _locker = new object();
        private readonly IConsole _console;

        public ManagerFactory(IConsole console)
        {
            _console = console;
        }

        public ManagerPair GetStrategyTradeManagerPair(Guid strategyId)
        {
            lock (_locker)
            {
                var sleepService = new SleepService();

                var apiClientFactory = new ApiClientFactory();
                var runnerService = new RunnerService(apiClientFactory);
                var orderPlacer = new OrderPlacer(apiClientFactory);
                var stakeCalculator = new ClosingStakeCalculator();

                var openingStakeProviderFactory = new OpeningStakeProviderFactory();
                var test = new List<string>()
                {
                    "255f49e7-28e7-4f2b-acb9-34a2898866f4",
                    "5331775e-fc1f-46c5-b665-f3d478b7d09f",
                    "984950bb-210e-4e4b-b76c-cf668fe40a6a"
                };

                if (test.Contains(strategyId.ToString()))
                {
                    return new ManagerPair(new TestOpeningOrderManager(), new TestClosingOrderManager());
                }

                var orderPriceFinder = new OrderPriceFinder(openingStakeProviderFactory);

                return new ManagerPair(new OpeningOrderManager(orderPlacer, runnerService, sleepService, orderPriceFinder), new ClosingOrderManager(sleepService, runnerService, orderPlacer, stakeCalculator));
            }
        }
    }
}