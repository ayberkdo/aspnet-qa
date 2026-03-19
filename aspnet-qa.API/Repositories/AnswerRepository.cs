using aspnet_qa.API.Models;

namespace aspnet_qa.API.Repositories
{
    public class AnswerRepository : GenericRepository<Answer>
    {
        public AnswerRepository(AppDbContext context) : base(context) { }
    }
}