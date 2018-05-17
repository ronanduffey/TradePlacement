using System;

namespace TradePlacement.Models
{
    public class Match
    {
        public Guid Id { get; set; }
        public Guid HomeTeam { get; set; }
        public FootballMatchModel FootballData { get; set; }
        public BetfairEvent BetfairData { get; set; }
        public MatchDetail WhoScoredData { get; set; }
    }
}