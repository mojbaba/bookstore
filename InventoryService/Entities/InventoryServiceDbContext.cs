using Microsoft.EntityFrameworkCore;

namespace InventoryService.Entities;

public class InventoryServiceDbContext : DbContext
{
    public InventoryServiceDbContext(DbContextOptions<InventoryServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<BookEntity> Books { get; set; }
}