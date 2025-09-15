using IceAndStone.API.Data;
using IceAndStone.API.Entities;
using IceAndStone.API.Repositories.Abstractions;

namespace IceAndStone.API.Repositories
{
    public class SessionRepository : BaseRepository<Session>, ISessionRepository
    {
        public SessionRepository(AppDbContext context) : base(context) { }
    }
}
