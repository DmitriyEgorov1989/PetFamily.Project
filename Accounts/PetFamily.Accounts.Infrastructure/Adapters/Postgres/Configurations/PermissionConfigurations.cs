using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Accounts.Core.Domain.Models;

namespace PetFamily.Accounts.Infrastructure.Adapters.Postgres.Configurations;

public class PermissionConfigurations : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions", "accounts");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("permission_id")
            .IsRequired();

        builder.Property(p => p.Code)
            .HasColumnName("code")
            .IsRequired();

    }
}