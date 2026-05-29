using Microsoft.EntityFrameworkCore;


namespace SmartLogisticsBackend.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      
    }

}