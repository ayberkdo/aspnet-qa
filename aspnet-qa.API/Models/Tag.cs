namespace aspnet_qa.API.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; } // yeni
        public ICollection<QuestionTag> QuestionTags { get; set; }
    }
}
