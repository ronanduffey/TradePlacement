using System.Collections.Generic;

namespace TradePlacement.Api
{
    public interface IApiClient
    {
        T GetData<T>(string method, Dictionary<string, object> args);
    }
}