namespace IceAndStone.API.Entities
{
    public class Team
    {
        public long Id { get; set; }
        public long GameId { get; set; }
        public string Name { get; set; } = "";
        public string Colour { get; set; } = "";
        public bool HasFirstRound { get; set; }
    }
}
