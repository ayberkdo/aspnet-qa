namespace aspnet_qa.API.DTOs
{
    public class AnswerDto : BaseDto
    {
        public string Content { get; set; }
        public bool IsAccepted { get; set; }
        public int Score { get; set; }
        public int QuestionId { get; set; }

        public string AppUserId { get; set; }
        public string AuthorName { get; set; }
    }
}