using Microsoft.AspNetCore.Identity;

namespace aspnet_qa.API.Models
{
    public class AppUser: IdentityUser
    {
        public string FullName { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
