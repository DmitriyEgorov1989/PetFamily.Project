using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Accounts.Core.Domain.Models.Accounts;
using PetFamily.SharedKernel.DomainModels.Ids;
using System.Text.Json;

namespace PetFamily.Accounts.Infrastructure.Adapters.Postgres.Configurations.AccountsConfigurations;

public class ParticipantConfigurations : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("participants", "accounts");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .IsRequired()
            .HasColumnName("id");

        builder.Property(p => p.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(p => p.FavoritePets)
            .HasColumnName("favorite_pets")
            .HasColumnType("jsonb")
            .HasConversion(
                favoritePets =>
                    JsonSerializer.Serialize(favoritePets, JsonSerializerOptions.Default),
                value =>
                    JsonSerializer.Deserialize<List<PetId>>(value, JsonSerializerOptions.Default)
                    ?? new List<PetId>());
    }
}