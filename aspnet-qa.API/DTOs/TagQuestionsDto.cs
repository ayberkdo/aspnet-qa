namespace aspnet_qa.API.DTOs
{
    public class TagQuestionsDto
    {
        public TagDto Tag { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }
}