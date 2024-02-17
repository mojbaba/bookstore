using Microsoft.EntityFrameworkCore;

namespace UserService
{
    public class UserServiceDbContext(DbContextOptions<UserServiceDbContext> options) : DbContext(options)
    {
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().HasIndex(u => u.Email).IsUnique();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}