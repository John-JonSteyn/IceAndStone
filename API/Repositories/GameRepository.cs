using IceAndStone.API.Data;
using IceAndStone.API.Entities;
using IceAndStone.API.Repositories.Abstractions;

namespace IceAndStone.API.Repositories
{
    public class GameRepository : BaseRepository<Game>, IGameRepository
    {
        public GameRepository(AppDbContext context) : base(context) { }
    }
}
