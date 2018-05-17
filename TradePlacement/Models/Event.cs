using System.Collections.Generic;

namespace TradePlacement.Models
{
    public class Event
    {
        public int EventId { get; internal set; }
        public List<Market> Markets { get; internal set; }
    }
}