namespace IceAndStone.API.Responses
{
    public record RoundResponse(long Id, long GameId, int Number, long? StartsFirstTeamId, DateTime? StartTime, DateTime? EndTime);
}
