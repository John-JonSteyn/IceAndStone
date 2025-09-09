namespace IceAndStone.API.Entities
{
    public class Achievement
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string TriggerType { get; set; } = "";
        public string? Description { get; set; }
    }
}
