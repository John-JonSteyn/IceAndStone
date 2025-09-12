namespace IceAndStone.API.Responses
{
    public record GameResponse(long Id, long SessionId, DateTime StartTime, DateTime? EndTime, int? TargetRounds);
}
