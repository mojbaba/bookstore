using Microsoft.EntityFrameworkCore;

namespace InventoryService.Entities;

public class InventoryServiceDbContext : DbContext
{
    public InventoryServiceDbContext(DbContextOptions<InventoryServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<BookEntity> Books { get; set; }
    
    public DbSet<BookHistoryEntity> BookHistories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookEntity>().HasMany(a => a.History).WithOne().HasForeignKey(a => a.BookId);
        modelBuilder.Entity<BookHistoryEntity>().HasIndex(a => a.OrderId);
    }
}