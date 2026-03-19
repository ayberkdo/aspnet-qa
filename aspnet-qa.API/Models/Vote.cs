namespace aspnet_qa.API.Models
{
    public class Vote:BaseEntity
    {
        public int Value { get; set; } // Sadece +1 (Upvote) veya -1 (Downvote) alacak

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        // Bir oy ya bir Soruya aittir ya da bir Cevaba. Bu yüzden ikisi de nullable (int?)
        public int? QuestionId { get; set; }
        public Question Question { get; set; }

        public int? AnswerId { get; set; }
        public Answer Answer { get; set; }
    }
}
