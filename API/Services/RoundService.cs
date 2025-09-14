using IceAndStone.API.Entities;
using IceAndStone.API.Repositories.Abstractions;
using IceAndStone.API.Requests;
using IceAndStone.API.Responses;
using IceAndStone.API.Services.Abstractions;

namespace IceAndStone.API.Services
{
    public class RoundService : IRoundService
    {
        private readonly IRoundRepository _rounds;
        private readonly IGameRepository _games;
        private readonly ITeamRepository _teams;

        public RoundService(IRoundRepository rounds, IGameRepository games, ITeamRepository teams)
        {
            _rounds = rounds;
            _games = games;
            _teams = teams;
        }

        public async Task<RoundResponse> StartAsync(StartRoundRequest request, CancellationToken cancellationToken)
        {
            var game = await _games.GetByIdAsync(request.GameId, cancellationToken)
                       ?? throw new KeyNotFoundException("Game not found.");
            if (game.EndTime is not null)
                throw new InvalidOperationException("Game already ended.");

            var startsTeam = await _teams.GetByIdAsync(request.StartsFirstTeamId, cancellationToken)
                             ?? throw new KeyNotFoundException("Starting team not found.");
            if (startsTeam.GameId != game.Id)
                throw new InvalidOperationException("Starting team does not belong to this game.");

            bool exists = await _rounds.AnyAsync(round => round.GameId == game.Id && round.Number == request.Number, cancellationToken);
            if (exists) throw new InvalidOperationException("Round number already exists for this game.");

            var round = new Round
            {
                GameId = game.Id,
                Number = request.Number,
                StartsFirstTeamId = startsTeam.Id,
                StartTime = DateTime.UtcNow
            };

            await _rounds.AddAsync(round, cancellationToken);
            await _rounds.SaveChangesAsync(cancellationToken);

            return new RoundResponse(round.Id, round.GameId, round.Number, round.StartsFirstTeamId, round.StartTime, round.EndTime);
        }

        public async Task<RoundResponse> EndAsync(EndRoundRequest request, CancellationToken cancellationToken)
        {
            var round = await _rounds.GetByIdAsync(request.RoundId, cancellationToken)
                        ?? throw new KeyNotFoundException("Round not found.");
            if (round.EndTime is not null)
                throw new InvalidOperationException("Round already ended.");

            round.EndTime = DateTime.UtcNow;
            await _rounds.SaveChangesAsync(cancellationToken);

            return new RoundResponse(round.Id, round.GameId, round.Number, round.StartsFirstTeamId, round.StartTime, round.EndTime);
        }
    }
}
