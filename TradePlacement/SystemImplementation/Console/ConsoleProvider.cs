namespace TradePlacement.SystemImplementation
{
    public class ConsoleProvider : IConsoleProvider
    {
        private readonly IConsole _instance = new Console();

        public IConsole GetInstance()
        {
            return _instance;
        }
    }
}