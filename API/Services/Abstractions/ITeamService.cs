using IceAndStone.API.Requests;
using IceAndStone.API.Responses;

namespace IceAndStone.API.Services.Abstractions
{
    public interface ITeamService
    {
        Task<(TeamResponse TeamA, TeamResponse TeamB)> CreatePairAsync(CreateTeamsRequest request, CancellationToken cancellationToken);
        Task AddPlayersAsync(AddPlayersRequest request, CancellationToken cancellationToken);
    }
}
