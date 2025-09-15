namespace IceAndStone.API.Responses
{
    public record TeamResponse(long Id, long GameId, string Name, string Colour, bool HasFirstRound);
}
