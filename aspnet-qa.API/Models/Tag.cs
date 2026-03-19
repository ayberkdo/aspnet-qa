namespace aspnet_qa.API.Models
{
    public class Tag:BaseEntity
    {
        public string Name { get; set; } // Örn: c#, asp.net-core
        public ICollection<QuestionTag> QuestionTags { get; set; }
    }
}
