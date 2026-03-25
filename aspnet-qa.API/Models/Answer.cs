namespace aspnet_qa.API.Models
{
    public class Answer : BaseEntity
    {
        public string Content { get; set; }
        public bool IsAccepted { get; set; } = false;

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
