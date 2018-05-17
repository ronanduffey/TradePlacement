using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TradePlacement.Models.Api;

namespace TradePlacement.Api
{
    public class BetfairLogin : IBetfairLogin
    {
        public string SessionToken { get; private set; }

        public WebRequest CreateRequestBase(string appKey)
        {
            var endpoint = "https://api.betfair.com/exchange/betting/json-rpc/v1";
            var CustomHeaders = new NameValueCollection
            {
                ["X-Application"] = appKey,
                ["X-Authentication"] = SessionToken
            };

            var _request = WebRequest.Create(new Uri(endpoint));
            _request.Method = "POST";
            _request.ContentType = "application/json-rpc";
            _request.Headers.Add(HttpRequestHeader.AcceptCharset, "ISO-8859-1,utf-8");
            _request.Headers.Add(CustomHeaders);
            return _request;
        }

        public async Task Login(string username, string password, string appKey)
        {
            var cert = new X509Certificate2("certificate.pfx", "");
            var clienthandler = new HttpClientHandler();
            clienthandler.ClientCertificates.Add(cert);
            var client = new HttpClient(clienthandler)
            {
                BaseAddress = new Uri("https://identitysso.betfair.com")
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("X-Application", appKey);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json-rpc"));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var postdata = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            };

            var result = await client.PostAsync("/api/certlogin", new FormUrlEncodedContent(postdata));
            result.EnsureSuccessStatusCode();

            var stream = new MemoryStream(result.Content.ReadAsByteArrayAsync().Result);
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var jsonResponse = reader.ReadToEnd();
                var loginDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginDetails>(jsonResponse);
                SessionToken = loginDetails.SessionToken;
            }
        }
    }
}