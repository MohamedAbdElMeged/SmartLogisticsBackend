using Microsoft.EntityFrameworkCore;
using SmartLogisticsBackend.Domain.Entities;

namespace SmartLogisticsBackend.Infrastructure.Persistence.Seeds;

public class DbSeed
{
    private readonly ApplicationDbContext _context;

    public DbSeed(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task SeedAsync(CancellationToken ct = default)
    {
        var roles = new List<Role>
        {
            new Role { Name = "customer" },
            new Role { Name = "admin" },
            new Role { Name = "driver" },
            
        };
        if (!await _context.Roles.AnyAsync())
        {
            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync(ct);
        }
        
        
    }
}