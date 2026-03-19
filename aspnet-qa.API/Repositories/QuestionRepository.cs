using aspnet_qa.API.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnet_qa.API.Repositories
{
    public class QuestionRepository : GenericRepository<Question>
    {
        public QuestionRepository(AppDbContext context) : base(context) { }

        public IQueryable<Question> GetQuestionsWithDetails()
        {
            return _dbSet
                .Include(q => q.AppUser)
                .Include(q => q.Answers)
                .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                .AsNoTracking();
        }
    }
}