using System;
using System.Collections.Generic;

namespace TradePlacement.Models
{
    public class BetfairEvent
    {
        public long Id { get; set; }
        public DateTime DateTime { get; set; }
        public List<Market> Markets { get; set; }
    }
}