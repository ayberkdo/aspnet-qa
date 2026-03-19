using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace aspnet_qa.API.Models
{
    public class AppDbContext:IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<QuestionTag> QuestionTags { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // QuestionTag tablosunun çift birincil anahtarı olduğunu belirtiyoruz
            builder.Entity<QuestionTag>()
                .HasKey(qt => new { qt.QuestionId, qt.TagId });

            // Bir kullanıcı silinirse, sorularının silinmesini engelliyoruz (NoAction) ki sistem çökmesin.
            builder.Entity<Question>()
                .HasOne(q => q.AppUser)
                .WithMany()
                .HasForeignKey(q => q.AppUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Answer>()
                .HasOne(a => a.AppUser)
                .WithMany()
                .HasForeignKey(a => a.AppUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
