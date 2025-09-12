namespace IceAndStone.API.Requests
{
    public record StartGameRequest(long SessionId, int? TargetRounds);
    public record EndGameRequest(long GameId);
}
