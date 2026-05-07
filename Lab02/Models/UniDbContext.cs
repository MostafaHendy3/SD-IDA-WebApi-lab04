using Microsoft.EntityFrameworkCore;

namespace Lab02.Models
{
    public class UniDbContext : DbContext
    {
        public UniDbContext(DbContextOptions<UniDbContext> options):base(options) { }
        
        public virtual DbSet<Student> Students { get; set; }
    }
}
