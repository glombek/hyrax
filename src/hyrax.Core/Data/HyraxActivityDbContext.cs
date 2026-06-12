using Microsoft.EntityFrameworkCore;
using hyrax.Core.ActivityPub.Models;

namespace hyrax.Core.ActivityPub.Services
{
    /// <summary>
    /// Entity Framework Core context for ActivityPub persistence.
    /// Manages Activity and Follower entities with schema in "hyrax".
    /// </summary>
    public class HyraxActivityDbContext : DbContext
    {
        public HyraxActivityDbContext(DbContextOptions<HyraxActivityDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet for ActivityPub activities.
        /// </summary>
        public DbSet<Activity> Activities { get; set; } = null!;

        /// <summary>
        /// DbSet for ActivityPub followers.
        /// </summary>
        public DbSet<Follower> Followers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Activity entity
            modelBuilder.Entity<Activity>(entity =>
            {
                entity.ToTable("Activities", "hyrax");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnType("nvarchar(450)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Actor)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Target)
                    .HasMaxLength(2000);

                entity.Property(e => e.RawJson)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.ProcessedAt)
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Received");

                entity.Property(e => e.RetryCount)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.Property(e => e.LastError)
                    .HasColumnType("nvarchar(max)");

                // Indexes for query performance
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.Actor, e.Status });
            });

            // Configure Follower entity
            modelBuilder.Entity<Follower>(entity =>
            {
                entity.ToTable("Followers", "hyrax");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnType("nvarchar(450)");

                entity.Property(e => e.ActorUri)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.LocalAuthorId)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.AcceptedAt)
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.Pending)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnType("datetimeoffset");

                entity.Property(e => e.InboxUrl)
                    .HasMaxLength(2000);

                // Indexes for query performance
                entity.HasIndex(e => new { e.ActorUri, e.LocalAuthorId })
                    .IsUnique();
                entity.HasIndex(e => new { e.LocalAuthorId, e.Pending });
            });
        }
    }
}
