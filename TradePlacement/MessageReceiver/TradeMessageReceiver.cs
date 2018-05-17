using TradePlacement.DataRepository.Raven;
using TradePlacement.Domain.Manager;
using TradePlacement.MessageProcessor;
using TradePlacement.Models;
using TradePlacement.SystemImplementation;
using TradePlacement.SystemImplementation.File;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TradePlacement.MessageReceiver
{
    public class TradeMessageReceiver
    {
        private readonly IConsole _console;
        private readonly IFile _file;

        public TradeMessageReceiver(IConsole console, IFile file)
        {
            _console = console;
            _file = file;
        }

        public void Start()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "suggested_trades", type: "direct");

                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queueName, "suggested_trades", "");

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            var body = ea.Body;
                            var trades = JsonConvert.DeserializeObject<List<TradeDetail>>(Encoding.UTF8.GetString(body));

                            foreach (var trade in trades)
                            {
                                _console.WriteLineWithTimestamp($"Received trade - {trade.Id.ToString()} - {trade.Match.HomeTeam} - {trade.MarketName} - {trade.RunnerName} - {trade.Side.ToString()}");
                            }

                            foreach (var trade in trades)
                            {
                                Task.Factory.StartNew(() => RunTrade(trade));
                            }
                        }
                        catch (Exception e)
                        {
                            _console.WriteLineWithTimestamp(e.Message);
                            _console.WriteLineWithTimestamp(e.StackTrace);
                        }
                    };

                    channel.BasicConsume(queueName, true, consumer);
                    _console.WriteLineWithTimestamp("Trade placer initialised.");

                    System.Console.ReadLine();
                }
            }
        }

        private async Task RunTrade(TradeDetail trade)
        {
            var messageProcessor = new TradeMessageProcessor(new ManagerFactory(_console), new TradeStore(), _console, _file);
            await messageProcessor.ProcessMessage(trade);
        }
    }
}