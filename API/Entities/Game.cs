namespace IceAndStone.API.Entities
{
    public class Game
    {
        public long Id { get; set; }
        public long SessionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? TargetRounds { get; set; }
    }
}
