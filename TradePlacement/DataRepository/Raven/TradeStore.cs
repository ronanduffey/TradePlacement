using TradePlacement.Models.Raven;

namespace TradePlacement.DataRepository.Raven
{
    public class TradeStore : ITradeStore
    {
        public void AddTrade(Trade trade)
        {
            var documentStores = new DocumentStores();
            using (var store = documentStores.GetTradesDocumentStore())
            {
                store.Initialize();
                using (var session = store.OpenSession())
                {
                    session.Store(trade);
                    session.SaveChanges();
                }
            }
        }
    }
}