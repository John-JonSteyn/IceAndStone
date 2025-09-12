using IceAndStone.API.Data;
using IceAndStone.API.Entities;
using IceAndStone.API.Repositories.Abstractions;

namespace IceAndStone.API.Repositories
{
    public class RoundRepository : BaseRepository<Round>, IRoundRepository
    {
        public RoundRepository(AppDbContext context) : base(context) { }
    }
}
