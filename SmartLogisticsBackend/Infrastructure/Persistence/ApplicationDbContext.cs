using Microsoft.EntityFrameworkCore;
using SmartLogisticsBackend.Domain.Models;

namespace SmartLogisticsBackend.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(builder =>
        {
            builder.OwnsOne(x => x.TotalPrice, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("TotalPriceAmount")
                    .HasPrecision(18, 2);

                money.Property(x => x.Currency)
                    .HasColumnName("TotalPriceCurrency")
                    .HasMaxLength(3);
            });
        });
    }

    public DbSet<Order> Orders { get; set; }
}