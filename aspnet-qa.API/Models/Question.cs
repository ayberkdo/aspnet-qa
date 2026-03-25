namespace aspnet_qa.API.Models
{
    public class Question : BaseEntity
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public int ViewCount { get; set; } = 0;

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public ICollection<Answer> Answers { get; set; }
        public ICollection<QuestionTag> QuestionTags { get; set; }
    }
}
