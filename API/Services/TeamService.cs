using IceAndStone.API.Entities;
using IceAndStone.API.Repositories.Abstractions;
using IceAndStone.API.Requests;
using IceAndStone.API.Responses;
using IceAndStone.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IceAndStone.API.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teams;
        private readonly IBaseRepository<Game> _games;
        private readonly IBaseRepository<Player> _players;

        public TeamService(ITeamRepository teams, IBaseRepository<Game> games, IBaseRepository<Player> players)
        {
            _teams = teams;
            _games = games;
            _players = players;
        }

        public async Task<(TeamResponse TeamA, TeamResponse TeamB)> CreatePairAsync(CreateTeamsRequest request, CancellationToken cancellationToken)
        {
            var game = await _games.GetByIdAsync(request.GameId, cancellationToken)
                       ?? throw new KeyNotFoundException("Game not found.");
            if (game.EndTime is not null)
                throw new InvalidOperationException("Cannot add teams to an ended game.");

            var existingTeams = await _teams.Query().Where(team => team.GameId == game.Id).ToListAsync(cancellationToken);
            if (existingTeams.Count > 0)
                throw new InvalidOperationException("Teams already exist for this game.");

            if (request.TeamAColour.Equals(request.TeamBColour, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Team colours must be different.");

            bool teamAStarts = request.FirstRoundTeam?.Equals("A", StringComparison.OrdinalIgnoreCase) == true;
            bool teamBStarts = request.FirstRoundTeam?.Equals("B", StringComparison.OrdinalIgnoreCase) == true;

            if (!teamAStarts && !teamBStarts) teamAStarts = true;

            if (teamAStarts && teamBStarts) throw new InvalidOperationException("Only one team can start first.");

            var teamA = new Team { GameId = game.Id, Name = request.TeamAName, Colour = request.TeamAColour, HasFirstRound = teamAStarts };
            var teamB = new Team { GameId = game.Id, Name = request.TeamBName, Colour = request.TeamBColour, HasFirstRound = teamBStarts };

            await _teams.AddRangeAsync(new[] { teamA, teamB }, cancellationToken);
            await _teams.SaveChangesAsync(cancellationToken);

            return (
                new TeamResponse(teamA.Id, teamA.GameId, teamA.Name, teamA.Colour, teamA.HasFirstRound),
                new TeamResponse(teamB.Id, teamB.GameId, teamB.Name, teamB.Colour, teamB.HasFirstRound)
            );
        }

        public async Task AddPlayersAsync(AddPlayersRequest request, CancellationToken cancellationToken)
        {
            var team = await _teams.GetByIdAsync(request.TeamId, cancellationToken)
                       ?? throw new KeyNotFoundException("Team not found.");

            foreach (var name in request.PlayerNames.Where(name => !string.IsNullOrWhiteSpace(name)))
            {
                await _players.AddAsync(new Player { TeamId = team.Id, Name = name.Trim() }, cancellationToken);
            }
            await _players.SaveChangesAsync(cancellationToken);
        }
    }
}
