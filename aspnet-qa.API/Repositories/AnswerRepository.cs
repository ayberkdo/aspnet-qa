using aspnet_qa.API.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnet_qa.API.Repositories
{
    public class AnswerRepository : GenericRepository<Answer>
    {
        public AnswerRepository(AppDbContext context) : base(context) { }

        public IQueryable<Answer> GetAnswersWithDetails()
        {
            return _dbSet
                .Include(a => a.AppUser)
                .Include(a => a.Question)
                .AsNoTracking();
        }
    }
}