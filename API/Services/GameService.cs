using IceAndStone.API.Entities;
using IceAndStone.API.Repositories.Abstractions;
using IceAndStone.API.Requests;
using IceAndStone.API.Responses;
using IceAndStone.API.Services.Abstractions;

namespace IceAndStone.API.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _games;
        private readonly IBaseRepository<Session> _sessions;

        public GameService(IGameRepository games, IBaseRepository<Session> sessions)
        {
            _games = games; _sessions = sessions;
        }

        public async Task<GameResponse> StartAsync(StartGameRequest request, CancellationToken cancellationToken)
        {
            var session = await _sessions.GetByIdAsync(request.SessionId, cancellationToken)
                          ?? throw new KeyNotFoundException("Session not found.");
            if (session.EndTime is not null) throw new InvalidOperationException("Cannot start a game in an ended session.");

            var game = new Game { SessionId = session.Id, StartTime = DateTime.UtcNow, TargetRounds = request.TargetRounds };
            await _games.AddAsync(game, cancellationToken);
            await _games.SaveChangesAsync(cancellationToken);

            return new GameResponse(game.Id, game.SessionId, game.StartTime, game.EndTime, game.TargetRounds);
        }

        public async Task<GameResponse> EndAsync(EndGameRequest request, CancellationToken cancellationToken)
        {
            var game = await _games.GetByIdAsync(request.GameId, cancellationToken)
                       ?? throw new KeyNotFoundException("Game not found.");
            if (game.EndTime is not null) throw new InvalidOperationException("Game already ended.");

            game.EndTime = DateTime.UtcNow;
            await _games.SaveChangesAsync(cancellationToken);

            return new GameResponse(game.Id, game.SessionId, game.StartTime, game.EndTime, game.TargetRounds);
        }
    }
}
