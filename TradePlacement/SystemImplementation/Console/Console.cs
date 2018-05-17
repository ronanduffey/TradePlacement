using System;

namespace TradePlacement.SystemImplementation
{
    public class Console : IConsole
    {
        public void WriteLineWithTimestamp(string message)
        {
            WriteLine($"{DateTime.UtcNow.ToLongTimeString()} - {message}");
        }

        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}