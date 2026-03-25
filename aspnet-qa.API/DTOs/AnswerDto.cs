namespace aspnet_qa.API.DTOs
{
    public class AnswerDto : BaseDto
    {
        public string Content { get; set; }
        public bool IsAccepted { get; set; }
        public int QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionSlug { get; set; }

        public string AppUserId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorPhotoUrl { get; set; }
    }
}