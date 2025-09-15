using IceAndStone.API.Data;
using IceAndStone.API.Entities;
using IceAndStone.API.Repositories.Abstractions;

namespace IceAndStone.API.Repositories
{
    public class TeamRepository : BaseRepository<Team>, ITeamRepository
    {
        public TeamRepository(AppDbContext context) : base(context) { }
    }
}
