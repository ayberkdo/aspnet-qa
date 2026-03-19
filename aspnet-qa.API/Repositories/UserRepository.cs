using aspnet_qa.API.Models;

namespace aspnet_qa.API.Repositories
{
    public class UserRepository : GenericRepository<AppUser>
    {
        public UserRepository(AppDbContext context) : base(context) { }
    }
}
