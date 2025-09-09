namespace IceAndStone.API.Entities
{
    public class Round
    {
        public long Id { get; set; }
        public long GameId { get; set; }
        public int Number { get; set; }
        public long? StartsFirstTeamId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
