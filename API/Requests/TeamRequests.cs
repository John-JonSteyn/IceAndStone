namespace IceAndStone.API.Requests
{
    public record CreateTeamsRequest(long GameId, string TeamAName, string TeamAColour, string TeamBName, string TeamBColour, string? FirstRoundTeam);
    public record AddPlayersRequest(long TeamId, List<string> PlayerNames);
}
