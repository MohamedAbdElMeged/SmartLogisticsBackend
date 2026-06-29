using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLogisticsBackend.Domain.Entities;

namespace SmartLogisticsBackend.Infrastructure.Persistence.Configurations;

public class RoleConfiguration: IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).HasMaxLength(50).IsRequired();

        builder.HasIndex(a => a.Name).IsUnique();
    }
}