using IceAndStone.API.Entities;
using IceAndStone.API.Repositories.Abstractions;
using IceAndStone.API.Requests;
using IceAndStone.API.Responses;
using IceAndStone.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IceAndStone.API.Services
{
    public class ScoreService : IScoreService
    {
        private readonly IScoreRepository _scores;
        private readonly IRoundRepository _rounds;
        private readonly ITeamRepository _teams;

        public ScoreService(IScoreRepository scores, IRoundRepository rounds, ITeamRepository teams)
        {
            _scores = scores; _rounds = rounds; _teams = teams;
        }

        public async Task<ScoreResponse> PostTeamScoreAsync(PostTeamScoreRequest request, CancellationToken cancellationToken)
        {
            if (request.Value < 0) throw new InvalidOperationException("Score cannot be negative.");

            var round = await _rounds.GetByIdAsync(request.RoundId, cancellationToken)
                        ?? throw new KeyNotFoundException("Round not found.");
            var team = await _teams.GetByIdAsync(request.TeamId, cancellationToken)
                       ?? throw new KeyNotFoundException("Team not found.");

            if (team.GameId != round.GameId)
                throw new InvalidOperationException("Team does not belong to this game/round.");

            var existing = await _scores.Query()
                .FirstOrDefaultAsync(score => score.RoundId == round.Id && score.TeamId == team.Id, cancellationToken);

            if (existing is null)
            {
                var score = new TeamScore { RoundId = round.Id, TeamId = team.Id, Value = request.Value };
                await _scores.AddAsync(score, cancellationToken);
                await _scores.SaveChangesAsync(cancellationToken);
                return new ScoreResponse(score.Id, score.RoundId, score.TeamId, score.Value);
            }
            else
            {
                existing.Value = request.Value;
                await _scores.SaveChangesAsync(cancellationToken);
                return new ScoreResponse(existing.Id, existing.RoundId, existing.TeamId, existing.Value);
            }
        }
    }
}
