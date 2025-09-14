using IceAndStone.API.Requests;
using IceAndStone.API.Responses;

namespace IceAndStone.API.Services.Abstractions
{
    public interface IScoreService
    {
        Task<ScoreResponse> PostTeamScoreAsync(PostTeamScoreRequest request, CancellationToken cancellationToken);
    }
}
