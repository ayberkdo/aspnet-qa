namespace aspnet_qa.API.Models
{
    public class Question : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int ViewCount { get; set; } = 0; // "En sık görülen" filtresi için
        public int Score { get; set; } = 0; // "En yüksek puan" filtresi için

        public string AppUserId { get; set; } // Soruyu soran kişi
        public AppUser AppUser { get; set; }

        public ICollection<Answer> Answers { get; set; }
        public ICollection<QuestionTag> QuestionTags { get; set; }
        public ICollection<Vote> Votes { get; set; }
    }
}
