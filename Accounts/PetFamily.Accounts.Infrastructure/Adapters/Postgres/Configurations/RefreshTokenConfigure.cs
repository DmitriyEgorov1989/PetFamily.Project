using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Accounts.Core.Domain.Models.Token;

namespace PetFamily.Accounts.Infrastructure.Adapters.Postgres.Configurations;

public class RefreshTokenConfigure : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens", "accounts");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(t => t.Token)
            .HasColumnName("token")
            .IsRequired();

        builder.Property(t => t.JwtId)
            .HasColumnName("jwt_id")
            .IsRequired();

        builder.Property(t => t.ExpiryDate)
            .HasColumnName("expiry_date")
            .IsRequired();

        builder.Property(t => t.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(t => t.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        builder.Property(t => t.Invalidated)
            .HasColumnName("invalidated")
            .IsRequired();

        builder.Property(t => t.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(t => t.Token)
            .IsUnique();

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId);

    }
}
