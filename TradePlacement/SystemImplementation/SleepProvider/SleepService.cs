using System.Threading;

namespace TradePlacement.SystemImplementation
{
    public class SleepService : ISleepService
    {
        public void Sleep(int millisecondsSleep)
        {
            Thread.Sleep(millisecondsSleep);
        }
    }
}
