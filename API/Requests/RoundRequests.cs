namespace IceAndStone.API.Requests
{
    public record StartRoundRequest(long GameId, int Number, long StartsFirstTeamId);
    public record EndRoundRequest(long RoundId);
}
