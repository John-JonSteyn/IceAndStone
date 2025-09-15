using IceAndStone.API.Entities;
using IceAndStone.API.Repositories.Abstractions;
using IceAndStone.API.Requests;
using IceAndStone.API.Responses;
using IceAndStone.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IceAndStone.API.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessions;
        private readonly IGameRepository _games;
        private readonly IBaseRepository<Lane> _lanes;

        public SessionService(ISessionRepository sessions, IGameRepository games, IBaseRepository<Lane> lanes)
        {
            _sessions = sessions; _games = games; _lanes = lanes;
        }

        public async Task<SessionResponse> StartAsync(StartSessionRequest request, CancellationToken cancellationToken)
        {
            bool laneExists = await _lanes.AnyAsync(lane => lane.Id == request.LaneId, cancellationToken);
            if (!laneExists) throw new InvalidOperationException("Lane does not exist.");

            var session = new Session { LaneId = request.LaneId, StartTime = DateTime.UtcNow };
            await _sessions.AddAsync(session, cancellationToken);
            await _sessions.SaveChangesAsync(cancellationToken);

            return new SessionResponse(session.Id, session.LaneId, session.StartTime, session.EndTime);
        }

        public async Task<SessionResponse> EndAsync(EndSessionRequest request, CancellationToken cancellationToken)
        {
            var session = await _sessions.GetByIdAsync(request.SessionId, cancellationToken)
                          ?? throw new KeyNotFoundException("Session not found.");
            if (session.EndTime is not null) throw new InvalidOperationException("Session already ended.");

            var openGames = await _games.Query()
                .Where(game => game.SessionId == session.Id && game.EndTime == null)
                .ToListAsync(cancellationToken);
            foreach (var game in openGames) game.EndTime = DateTime.UtcNow;

            session.EndTime = DateTime.UtcNow;
            await _sessions.SaveChangesAsync(cancellationToken);

            return new SessionResponse(session.Id, session.LaneId, session.StartTime, session.EndTime);
        }
    }
}
