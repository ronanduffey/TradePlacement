namespace TradePlacement.SystemImplementation
{
    public interface IConsole
    {
        void WriteLine(string message);

        void WriteLineWithTimestamp(string message);
    }
}