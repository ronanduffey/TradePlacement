using TradePlacement.DataRepository.Raven;
using TradePlacement.Domain.Exceptions;
using TradePlacement.Domain.Manager;
using TradePlacement.Models;
using TradePlacement.Models.Raven;
using TradePlacement.SystemImplementation;
using TradePlacement.SystemImplementation.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TradePlacement.MessageProcessor
{
    public class TradeMessageProcessor : ITradeMessageProcessor
    {
        private readonly IManagerFactory _managerFactory;
        private readonly ITradeStore _tradeStore;
        private readonly IConsole _console;
        private readonly IFile _file;

        public TradeMessageProcessor(IManagerFactory managerFactory, ITradeStore tradeStore, IConsole console, IFile file)
        {
            _managerFactory = managerFactory;
            _tradeStore = tradeStore;
            _console = console;
            _file = file;
        }

        public async Task ProcessMessage(TradeDetail trade)
        {
            try
            {
                CopyMatchMetadata(trade);
                var strategyTradeManagers = _managerFactory.GetStrategyTradeManagerPair(trade.StrategyId);

                var openingOrderSummary = await strategyTradeManagers.OpeningOrderManager.PlaceOpeningOrder(trade, new List<TradeDetail>());
                await strategyTradeManagers.ClosingOrderManager.ManageCloseout(openingOrderSummary);

                var databaseTradeRecord = BuildTradeRecord(trade, "COMPLETE", null);
                databaseTradeRecord.OpeningId = openingOrderSummary.OpenOrderResponse.BetId;

                WriteToDatabase(databaseTradeRecord);
                _console.WriteLineWithTimestamp($"Trade complete - {trade.Id}");
            }
            catch (OrderCancelledException e)
            {
                _console.WriteLineWithTimestamp(e.Message);
                _console.WriteLineWithTimestamp($"Trade cancelled - {trade.Id}");

                var databaseTradeRecord = BuildTradeRecord(trade, e.ExceptionCode, null);
                databaseTradeRecord.OpeningId = e.BetId;

                WriteToDatabase(databaseTradeRecord);
            }
            catch (NoPriceWithRequiredStakeAvailableException e)
            {
                HandleTradeException(trade, e);
            }
            catch (OrderActionErrorException e)
            {
                HandleTradeException(trade, e);
            }
            catch (MarketSuspendedException e)
            {
                HandleTradeException(trade, e);
            }
            catch (InsufficientFundsException e)
            {
                HandleTradeException(trade, e);
            }
            catch (Exception e)
            {
                _console.WriteLineWithTimestamp(e.Message);
                _console.WriteLineWithTimestamp(e.StackTrace);
                _console.WriteLineWithTimestamp($"Trade failed - {trade.Id}");

                var databaseTradeRecord = BuildTradeRecord(trade, "ERROR", e.Message);
                WriteToDatabase(databaseTradeRecord);
            }
        }

        private void HandleTradeException(TradeDetail trade, OrderException tradeException)
        {
            _console.WriteLineWithTimestamp(tradeException.Message);
            _console.WriteLineWithTimestamp($"Trade failed - {trade.Id}");

            var databaseTradeRecord = BuildTradeRecord(trade, tradeException.ExceptionCode, tradeException.Message);
            WriteToDatabase(databaseTradeRecord);
        }

        private Trade BuildTradeRecord(TradeDetail trade, string status, string message)
        {
            return new Trade()
            {
                Id = trade.Id,
                MarketName = trade.MarketName,
                Match = trade.Match,
                Notes = message,
                RunnerName = trade.RunnerName,
                Side = trade.Side,
                Status = status,
                StrategyId = trade.StrategyId,
                Tracking = trade.Tracking,
                TrackingTradeOffset = trade.TrackingTradeOffset
            };
        }

        private void WriteToDatabase(Trade trade)
        {
            _tradeStore.AddTrade(trade);
        }

        private void CopyMatchMetadata(TradeDetail trade)
        {
            var fileName = new FileInfo(trade.Match.WhoScoredData.MatchMetadataLocation).Name;
            _file.Copy(trade.Match.WhoScoredData.MatchMetadataLocation, @"C:/Users/Cobalt4/TradeDecisionData/" + fileName);
        }
    }
}