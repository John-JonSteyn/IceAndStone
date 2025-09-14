using IceAndStone.API.Requests;
using IceAndStone.API.Responses;

namespace IceAndStone.API.Services.Abstractions
{
    public interface IRoundService
    {
        Task<RoundResponse> StartAsync(StartRoundRequest request, CancellationToken cancellationToken);
        Task<RoundResponse> EndAsync(EndRoundRequest request, CancellationToken cancellationToken);
    }
}
