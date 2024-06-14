using Microsoft.EntityFrameworkCore;
using LoadingAPI.Models;

namespace LoadingAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Session>()
                .OwnsMany(s => s.Scenarios, a =>
                {
                    a.WithOwner().HasForeignKey("SessionId");
                    a.Property<int>("Id");
                    a.HasKey("Id");
                });

            modelBuilder.Entity<Session>()
                .Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Session>()
                .Property(s => s.CurrentOption)
                .HasMaxLength(100);

            modelBuilder.Entity<Vote>()
                .Property(v => v.UserId)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Vote>()
                .Property(v => v.Choice)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Review>()
                .Property(r => r.UserId)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Review>()
                .Property(r => r.Text)
                .IsRequired()
                .HasMaxLength(1000);

            modelBuilder.Entity<Review>()
                .Property(r => r.SubmittedAt)
                .IsRequired();
        }
    }
}
