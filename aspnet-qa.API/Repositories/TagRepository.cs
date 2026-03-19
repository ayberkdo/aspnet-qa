using aspnet_qa.API.Models;

namespace aspnet_qa.API.Repositories
{
    public class TagRepository : GenericRepository<Tag>
    {
        public TagRepository(AppDbContext context) : base(context) { }
    }
}