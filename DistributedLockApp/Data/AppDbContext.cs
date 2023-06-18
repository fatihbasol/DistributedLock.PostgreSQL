using DistributedLockApp.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace DistributedLockApp.Data
{
    public class AppDbContext : DbContext
    {

        public static readonly string CONN_STR = "User ID=postgres;Password=1234;Server=localhost;Port=5432;Database=DistrubutedLockPocDB";

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(CONN_STR);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(f => f.Id).ValueGeneratedOnAdd();
                entity.Property(f => f.Name).HasMaxLength(100);
            });
        }
    }
}
