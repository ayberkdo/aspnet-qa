namespace aspnet_qa.API.DTOs
{
    public class VoteDto : BaseDto
    {
        public int Value { get; set; } // +1 veya -1
        public string AppUserId { get; set; }
        public int? QuestionId { get; set; }
        public int? AnswerId { get; set; }
    }
}