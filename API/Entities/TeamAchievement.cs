namespace IceAndStone.API.Entities
{
    public class TeamAchievement
    {
        public long Id { get; set; }
        public long AchievementId { get; set; }
        public long TeamId { get; set; }
        public long GameId { get; set; }
        public DateTime AchievedAt { get; set; }
    }
}
