namespace aspnet_qa.API.Models
{
    public class Answer : BaseEntity
    {
        public string Content { get; set; }
        public bool IsAccepted { get; set; } = false; // Soruyu soran bu cevabı onayladı mı?
        public int Score { get; set; } = 0;

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public string AppUserId { get; set; } // Cevabı yazan kişi
        public AppUser AppUser { get; set; }

        public ICollection<Vote> Votes { get; set; }
    }
}
