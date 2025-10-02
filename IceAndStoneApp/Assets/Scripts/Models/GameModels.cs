using System;
using System.Collections.Generic;

namespace IceAndStone.App.Models
{
    [Serializable]
    public sealed class StartGameRequest
    {
        public long SessionId { get; set; }
        public int? TargetRounds { get; set; }
    }

    [Serializable]
    public sealed class EndGameRequest
    {
        public long GameId { get; set; }
    }

    [Serializable]
    public sealed class GameResponse
    {
        public long Id { get; set; }
        public long SessionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? TargetRounds { get; set; }
    }
}
