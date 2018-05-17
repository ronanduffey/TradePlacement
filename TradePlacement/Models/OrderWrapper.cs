using TradePlacement.Models.Api;

namespace TradePlacement.Models
{
    public class OrderWrapper
    {
        public string MarketId { get; }
        public long SelectionId { get; }
        public Side Side { get; }
        public OrderTick OrderTick { get; }
        public PersistenceType Persistencetype { get; }

        public string BetId { get; private set; }

        public OrderWrapper(string marketId, long selectionId, Side side, OrderTick price, PersistenceType persistancetype)
        {
            MarketId = marketId;
            SelectionId = selectionId;
            Side = side;
            OrderTick = price;
            Persistencetype = persistancetype;
        }

        public void AddBetId(string betId)
        {
            if (!string.IsNullOrEmpty(BetId))
            {
                throw new System.Exception();
            }

            BetId = betId;
        }
    }
}