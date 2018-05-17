using TradePlacement.Models;
using TradePlacement.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TradePlacement.Domain.StakeProviders.Closing
{
    public class ClosingStakeCalculator : IClosingStakeCalculator
    {
        public KeyValuePair<Side, double> GetFullHedgeStake(List<Order> runnerOrders, double currentPrice)
        {
            var backOrders = runnerOrders.Where(x => x.Side == Side.BACK).ToList();
            var layOrders = runnerOrders.Where(x => x.Side == Side.LAY).ToList();

            var totalBackStaked = backOrders.Sum(x => x.SizeMatched);
            var totalLayStaked = layOrders.Sum(x => x.SizeMatched);

            if (totalBackStaked > 0 && totalLayStaked > 0)
            {
                var averageBackOdds = Convert.ToDouble((backOrders.Sum(x => x.SizeMatched * x.Price)) / totalBackStaked);
                var averageLayOdds = Convert.ToDouble((layOrders.Sum(x => x.SizeMatched * x.Price)) / totalLayStaked);

                var backBetWinnings = totalBackStaked * (averageBackOdds - 1);
                var layBetLosses = totalLayStaked * (averageLayOdds - 1);

                var backBetLosses = -totalBackStaked;
                var layBetWinnings = totalLayStaked;

                var netWinOutcome = backBetWinnings - layBetLosses;
                var netLossOutcome = backBetLosses + layBetWinnings;

                var netProfitLossPosition = Math.Abs(Convert.ToDouble(netWinOutcome)) + Math.Abs(Convert.ToDouble(netLossOutcome));
                var hedgePortion = Math.Round(netProfitLossPosition / currentPrice, 2);
                var hedgeSide = totalBackStaked > totalLayStaked ? Side.LAY : Side.BACK;
                return hedgePortion > 2 ? new KeyValuePair<Side, double>(hedgeSide, hedgePortion) : new KeyValuePair<Side, double>(Side.NONE, 0);
            }

            if (totalBackStaked > 0)
            {
                var averageBackOdds = backOrders.Sum(x => x.SizeMatched * x.Price) / totalBackStaked;
                var averageLayOdds = backOrders.Sum(x => x.SizeMatched * currentPrice) / totalBackStaked;

                var backBetWinnings = totalBackStaked * (averageBackOdds - 1);
                var layBetLosses = totalBackStaked * (averageLayOdds - 1);

                var backBetLosses = -totalBackStaked;
                var layBetWinnings = totalBackStaked;

                var netWinOutcome = backBetWinnings - layBetLosses;
                var netLossOutcome = backBetLosses + layBetWinnings;

                var netProfitLossPosition = Convert.ToDouble(netWinOutcome) + Convert.ToDouble(netLossOutcome);
                var hedgePortion = netProfitLossPosition / currentPrice;
                var totalHedgeStake = Math.Round(Convert.ToDouble(totalBackStaked) + hedgePortion, 2);
                return totalHedgeStake > 2 ? new KeyValuePair<Side, double>(Side.LAY, totalHedgeStake) : new KeyValuePair<Side, double>(Side.NONE, 0);
            }

            if (totalLayStaked > 0)
            {
                var averageBackOdds = layOrders.Sum(x => x.SizeMatched * currentPrice) / totalLayStaked;
                var averageLayOdds = layOrders.Sum(x => x.SizeMatched * x.Price) / totalLayStaked;

                var backBetWinnings = totalLayStaked * (averageBackOdds - 1);
                var layBetLosses = totalLayStaked * (averageLayOdds - 1);

                var backBetLosses = -totalLayStaked;
                var layBetWinnings = totalLayStaked;

                var netWinOutcome = backBetWinnings - layBetLosses;
                var netLossOutcome = backBetLosses + layBetWinnings;

                var netProfitLossPosition = Convert.ToDouble(netWinOutcome) + Convert.ToDouble(netLossOutcome);
                var hedgePortion = netProfitLossPosition / currentPrice;
                var totalHedgeStake = Math.Round(Convert.ToDouble(totalLayStaked) - hedgePortion, 2);
                return totalHedgeStake > 2 ? new KeyValuePair<Side, double>(Side.BACK, totalHedgeStake) : new KeyValuePair<Side, double>(Side.NONE, 0);
            }

            throw new InvalidOperationException("The given orders do not have matched BACK or LAY amounts");
        }
    }
}