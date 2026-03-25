namespace aspnet_qa.API.DTOs
{
    public class QuestionDto : BaseDto
    {
        public string Title { get; set; }
        public string Slug { get; set; } // yeni
        public string Content { get; set; }
        public int ViewCount { get; set; }

        public string AppUserId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorPhotoUrl { get; set; }

        public int AnswerCount { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}