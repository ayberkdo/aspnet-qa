using aspnet_qa.API.DTOs;
using aspnet_qa.API.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnet_qa.API.Repositories
{
    public class TagRepository : GenericRepository<Tag>
    {
        public TagRepository(AppDbContext context) : base(context) { }

        public async Task<TagQuestionsDto?> GetTagWithQuestionsBySlugAsync(string slug)
        {
            return await _dbSet
                .Where(t => t.Slug == slug)
                .AsNoTracking()
                .Select(t => new TagQuestionsDto
                {
                    Tag = new TagDto
                    {
                        Name = t.Name,
                        Slug = t.Slug
                    },
                    Questions = t.QuestionTags
                        .Select(qt => qt.Question)
                        .OrderByDescending(q => q.Created)
                        .Select(q => new QuestionDto
                        {
                            Title = q.Title,
                            Slug = q.Slug,
                            Content = q.Content,
                            ViewCount = q.ViewCount,
                            AppUserId = q.AppUserId,
                            AuthorName = q.AppUser != null ? q.AppUser.FullName : "Kullanıcı",
                            AuthorUserName = q.AppUser != null ? q.AppUser.UserName : "kullanici",
                            AuthorPhotoUrl = q.AppUser != null ? q.AppUser.PhotoUrl : "default-profile-photo.jpg",
                            AnswerCount = q.Answers.Count,
                            Tags = q.QuestionTags
                                .Select(qtt => new TagDto
                                {
                                    Name = qtt.Tag.Name,
                                    Slug = qtt.Tag.Slug
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}