using Microsoft.EntityFrameworkCore;

namespace OrderService.Entities;

public class OrderServiceDbContext : DbContext
{
    public OrderServiceDbContext(DbContextOptions<OrderServiceDbContext> options) : base(options)
    {
    }

    public DbSet<OrderEntity> Orders { get; set; } = null!;
    public DbSet<OrderItemEntity> OrderItems { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderEntity>().HasMany(x => x.Items).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}