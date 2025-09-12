namespace IceAndStone.API.Responses
{
    public record SessionResponse(long Id, long LaneId, DateTime StartTime, DateTime? EndTime);
}
