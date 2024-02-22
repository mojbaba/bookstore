using Microsoft.EntityFrameworkCore;

namespace TokenService.Entities;

public class BookPurchaseTokenDbContext : DbContext
{
    public BookPurchaseTokenDbContext(DbContextOptions<BookPurchaseTokenDbContext> options) : base(options)
    {
    }

    public BookPurchaseTokenDbContext()
    {
    }

    public virtual DbSet<BookPurchaseTokenEntity> Tokens { get; set; }
    public virtual DbSet<BookPurchaseTokenHistoryEntity> History { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}