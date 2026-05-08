using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Accounts.Core.Domain.Models.Accounts;
using PetFamily.Accounts.Core.Domain.Models.VO;
using PetFamily.SharedKernel.DomainModels.VO;
using System.Text.Json;

namespace PetFamily.Accounts.Infrastructure.Adapters.Postgres.Configurations.AccountsConfigurations;

public class VolunteerConfigurations : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("volunteers", "accounts");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .IsRequired()
            .HasColumnName("volunteer_id");

        builder.Property(v => v.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(v => v.HelpRequisites)
            .HasColumnName("help_requisites")
            .HasColumnType("jsonb")
            .HasConversion(
                helpRequisites =>
                    JsonSerializer.Serialize(helpRequisites, JsonSerializerOptions.Default),
                value =>
                    JsonSerializer.Deserialize<List<HelpRequisite>>(value, JsonSerializerOptions.Default)
                    ?? new List<HelpRequisite>());

        builder.Property(v => v.Experience)
            .HasConversion(
                experience => experience.Year,
                value => Experience.Create(value).Value);
    }
}