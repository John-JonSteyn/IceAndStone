using IceAndStone.API.Requests;
using IceAndStone.API.Responses;

namespace IceAndStone.API.Services.Abstractions
{
    public interface ISessionService
    {
        Task<SessionResponse> StartAsync(StartSessionRequest request, CancellationToken cancellationToken);
        Task<SessionResponse> EndAsync(EndSessionRequest request, CancellationToken cancellationToken);
    }
}
