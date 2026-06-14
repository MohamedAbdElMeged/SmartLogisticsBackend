using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLogisticsBackend.Domain.Entities;

namespace SmartLogisticsBackend.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);
 
        builder.Property(a => a.Role)       .HasMaxLength(50);
        builder.Property(a => a.Method)     .HasMaxLength(10)  .IsRequired();
        builder.Property(a => a.Path)       .HasMaxLength(500) .IsRequired();
        builder.Property(a => a.IpAddress)  .HasMaxLength(45)  .IsRequired();
        builder.Property(a => a.UserAgent)  .HasMaxLength(500);
        
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.CreatedAt);
        builder.HasIndex(a => a.StatusCode);
    }
}
