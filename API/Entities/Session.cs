namespace IceAndStone.API.Entities
{
    public class Session
    {
        public long Id { get; set; }
        public long LaneId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
