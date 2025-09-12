namespace IceAndStone.API.Requests
{
    public record PostTeamScoreRequest(long RoundId, long TeamId, int Value);
}
