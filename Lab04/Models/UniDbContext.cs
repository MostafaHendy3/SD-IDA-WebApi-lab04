using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lab04.Models
{
    public class UniDbContext : IdentityDbContext<ApplicationUser>
    {
        public UniDbContext(DbContextOptions<UniDbContext> options)
            : base(options) { }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(e => e.TokenHash).IsUnique();
                entity.Property(e => e.TokenHash).HasMaxLength(64);
                entity.Property(e => e.ReplacedByHash).HasMaxLength(64);
                entity
                    .HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
