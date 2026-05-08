using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.Accounts.Core.Domain.Models.Accounts;
using PetFamily.SharedKernel.DomainModels.VO;
using System.Text.Json;

namespace PetFamily.Accounts.Infrastructure.Adapters.Postgres.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "accounts");

        builder.Property(u => u.RoleId)
            .HasColumnName("role_id")
            .IsRequired();

        builder.Property(u => u.Photos)
            .HasColumnName("photos")
            .HasColumnType("jsonb")
            .HasConversion(
                photos => JsonSerializer.Serialize(photos, JsonSerializerOptions.Default),
                value => JsonSerializer.Deserialize<List<Photo>>(value, JsonSerializerOptions.Default)
                         ?? new List<Photo>());

        builder.Property(u => u.SocialNetwork)
            .HasColumnName("social_networks")
            .HasColumnType("jsonb")
            .HasConversion(
                socialNetworks => JsonSerializer.Serialize(socialNetworks,
                    JsonSerializerOptions.Default),
                value => JsonSerializer.Deserialize<List<SocialNetwork>>(value,
                             JsonSerializerOptions.Default)
                         ?? new List<SocialNetwork>());

        builder.ComplexProperty(u => u.FullName, fn =>
        {
            fn.IsRequired();
            fn.Property(fn => fn.FirstName)
                .HasMaxLength(FullName.MAX_LENGTH_FULLNAME)
                .HasColumnName("first_name")
                .IsRequired();

            fn.Property(fn => fn.MiddleName)
                .HasMaxLength(FullName.MAX_LENGTH_FULLNAME)
                .HasColumnName("middle_name")
                .IsRequired();

            fn.Property(fn => fn.LastName)
                .HasMaxLength(FullName.MAX_LENGTH_FULLNAME)
                .HasColumnName("last_name")
                .IsRequired();
        });

        builder.HasOne(u => u.Admin)
            .WithOne(a => a.User)
            .HasForeignKey<Admin>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.Participant)
            .WithOne(p => p.User)
            .HasForeignKey<Participant>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.Volunteer)
            .WithOne(v => v.User)
            .HasForeignKey<Volunteer>(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}