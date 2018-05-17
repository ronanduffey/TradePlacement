using TradePlacement.Models;
using System.Threading.Tasks;

namespace TradePlacement.MessageProcessor
{
    public interface ITradeMessageProcessor
    {
        Task ProcessMessage(TradeDetail trade);
    }
}