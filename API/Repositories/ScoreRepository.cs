using IceAndStone.API.Data;
using IceAndStone.API.Entities;
using IceAndStone.API.Repositories.Abstractions;

namespace IceAndStone.API.Repositories
{
    public class ScoreRepository : BaseRepository<TeamScore>, IScoreRepository
    {
        public ScoreRepository(AppDbContext context) : base(context) { }
    }
}
