using System;
using Amazon.KeyManagementService;
using Amazon;
using Amazon.KeyManagementService.Model;
using System.IO;

namespace TradePlacement.Models
{
    public sealed class Credentials
    {
        private static Credentials instance = null;
        private static readonly object padlock = new object();

        public string Username { get; }
        public string Password { get; }
        public string AppKey { get; }

        private Credentials(string username, string password, string appKey)
        {
            Username = username;
            Password = password;
            AppKey = appKey;
        }

        public static Credentials Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        var username = DecryptSecret("REDACTED");
                        var password = DecryptSecret("REDACTED");
                        var appKey = DecryptSecret("REDACTED");
                        instance = new Credentials(username, password, appKey);
                    }
                    return instance;
                }
            }
        }

        private static string DecryptSecret(string encryptedSecret)
        {
            var encryptedKeyAsByteArray = Convert.FromBase64String(encryptedSecret);

            using (var memoryStream = new MemoryStream(encryptedKeyAsByteArray))
            {
                using (var kmsClient = new AmazonKeyManagementServiceClient(RegionEndpoint.EUWest1))
                {
                    var decrypted = kmsClient.DecryptAsync(new DecryptRequest()
                    {
                        CiphertextBlob = memoryStream,
                    }).GetAwaiter().GetResult();

                    using (var streamReader = new StreamReader(decrypted.Plaintext))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }
}