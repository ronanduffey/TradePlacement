using Raven.Client;
using Raven.Client.Document;

namespace TradePlacement.DataRepository.Raven
{
    public class DocumentStores
    {
        public IDocumentStore GetTradesDocumentStore()
        {
            return new DocumentStore
            {
                Url = "http://localhost:8080/",
                DefaultDatabase = "Trades"
            };
        }
    }
}