using TradePlacement.Models;
using System.Threading.Tasks;

namespace TradePlacement.Domain.Manager
{
    public interface IClosingOrderManager
    {
        Task ManageCloseout(OpenOrderResult openingOrderSummary);
    }
}