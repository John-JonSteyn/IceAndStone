using IceAndStone.API.Requests;
using IceAndStone.API.Responses;

namespace IceAndStone.API.Services.Abstractions
{
    public interface IGameService
    {
        Task<GameResponse> StartAsync(StartGameRequest request, CancellationToken cancellationToken);
        Task<GameResponse> EndAsync(EndGameRequest request, CancellationToken cancellationToken);
    }
}
