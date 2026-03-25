using aspnet_qa.API.Helpers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace aspnet_qa.API.Models
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<QuestionTag> QuestionTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<QuestionTag>()
                .HasKey(qt => new { qt.QuestionId, qt.TagId });

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

            builder.Entity<Question>()
                .Property(x => x.Slug)
                .HasMaxLength(220);

            builder.Entity<Tag>()
                .Property(x => x.Slug)
                .HasMaxLength(220);

            builder.Entity<Question>()
                .HasIndex(x => x.Slug)
                .IsUnique()
                .HasFilter("[Slug] IS NOT NULL AND [Slug] <> ''");

            builder.Entity<Tag>()
                .HasIndex(x => x.Slug)
                .IsUnique()
                .HasFilter("[Slug] IS NOT NULL AND [Slug] <> ''");
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await ApplySlugsAsync(cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task ApplySlugsAsync(CancellationToken cancellationToken)
        {
            var questionEntries = ChangeTracker.Entries<Question>()
                .Where(e =>
                    e.State == EntityState.Added ||
                    (e.State == EntityState.Modified &&
                     !string.Equals(
                         e.Property(x => x.Title).OriginalValue,
                         e.Property(x => x.Title).CurrentValue,
                         StringComparison.Ordinal)));

            foreach (var entry in questionEntries)
            {
                var baseSlug = SlugHelper.Generate(entry.Entity.Title);
                var candidate = baseSlug;
                var counter = 2;

                while (await Questions.AnyAsync(x => x.Id != entry.Entity.Id && x.Slug == candidate, cancellationToken))
                {
                    candidate = $"{baseSlug}-{counter}";
                    counter++;
                }

                entry.Entity.Slug = candidate;
            }

            var tagEntries = ChangeTracker.Entries<Tag>()
                .Where(e =>
                    e.State == EntityState.Added ||
                    (e.State == EntityState.Modified &&
                     !string.Equals(
                         e.Property(x => x.Name).OriginalValue,
                         e.Property(x => x.Name).CurrentValue,
                         StringComparison.Ordinal)));

            foreach (var entry in tagEntries)
            {
                var baseSlug = SlugHelper.Generate(entry.Entity.Name);
                var candidate = baseSlug;
                var counter = 2;

                while (await Tags.AnyAsync(x => x.Id != entry.Entity.Id && x.Slug == candidate, cancellationToken))
                {
                    candidate = $"{baseSlug}-{counter}";
                    counter++;
                }

                entry.Entity.Slug = candidate;
            }
        }
    }
}
