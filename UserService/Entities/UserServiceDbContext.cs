using Microsoft.EntityFrameworkCore;

namespace UserService
{
    public class UserServiceDbContext : DbContext
    {

        public UserServiceDbContext(DbContextOptions<UserServiceDbContext> options) : base(options)
        {
        }

        public UserServiceDbContext()
        {
        }
        
        public virtual DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().HasIndex(u => u.Email).IsUnique();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}