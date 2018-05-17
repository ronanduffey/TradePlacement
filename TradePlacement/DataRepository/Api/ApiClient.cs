using TradePlacement.Models;
using TradePlacement.Models.Api;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace TradePlacement.Api
{
    public class ApiClient : IApiClient
    {
        private readonly object _locker = new object();
        private readonly IBetfairLogin _betfairLogin;

        public ApiClient(IBetfairLogin betfairLogin)
        {
            _betfairLogin = betfairLogin;
        }

        public T GetData<T>(string method, Dictionary<string, object> args)
        {
            var credentials = Credentials.Instance;
            var baseRequest = _betfairLogin.CreateRequestBase(credentials.AppKey);
            ServicePointManager.Expect100Continue = false;
            return GetResults<T>(baseRequest, method, args);
        }

        private T GetResults<T>(WebRequest request, string method, Dictionary<string, object> args)
        {
            ServicePointManager.Expect100Continue = false;
            using (Stream stream = request.GetRequestStream())
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                var call = new JsonRequest { Method = method, Id = 1, Params = args };
                Export(call, writer);
            }
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var jsonResponse = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(jsonResponse);
            }
        }

        private static JsonResponse<T> Import<T>(TextReader reader)
        {
            var jsonResponse = reader.ReadToEnd();
            return Deserialize<JsonResponse<T>>(jsonResponse);
        }

        private static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        private static void Export(JsonRequest request, TextWriter writer)
        {
            var json = Serialize(request);
            writer.Write(json);
        }

        private static string Serialize<T>(T value)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(value, settings);
        }
    }
}