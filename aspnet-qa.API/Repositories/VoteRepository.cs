using aspnet_qa.API.Models;

namespace aspnet_qa.API.Repositories
{
    public class VoteRepository : GenericRepository<Vote>
    {
        public VoteRepository(AppDbContext context) : base(context) { }
    }
}