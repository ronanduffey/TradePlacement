using System.Net;
using System.Threading.Tasks;

namespace TradePlacement.Api
{
    public interface IBetfairLogin
    {
        string SessionToken { get; }

        WebRequest CreateRequestBase(string appKey);

        Task Login(string username, string password, string appKey);
    }
}