namespace IceAndStone.API.Entities
{
    public class TeamScore
    {
        public long Id { get; set; }
        public long RoundId { get; set; }
        public long TeamId { get; set; }
        public int Value { get; set; }
    }
}
