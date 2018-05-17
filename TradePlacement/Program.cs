using TradePlacement.MessageReceiver;
using TradePlacement.SystemImplementation;
using TradePlacement.SystemImplementation.File;

namespace TradePlacement
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var consoleProvider = new ConsoleProvider();
            var console = consoleProvider.GetInstance();
            var file = new File();

            var messageReceiver = new TradeMessageReceiver(console, file);
            messageReceiver.Start();
        }
    }
}