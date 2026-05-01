using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using System.Text.Json;
using Email = PetFamily.Core.Domain.Models.SharedKernel.VO.Email;

namespace PetFamily.Infrastructure.Adapters.Postgres.WriteDataBase.Configurations;

public class VolunteersConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("volunteers");

        builder.HasKey(x => x.Id);

        builder.Property(v => v.Id)
            .HasColumnName("volunteer_id")
            .ValueGeneratedNever()
            .HasConversion(id => id.Id,
                value => VolunteerId.Create(value).Value)
            .IsRequired();

        builder.ComplexProperty(v => v.FullName, fn =>
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

        builder.Property(v => v.Email)
            .HasConversion(e => e.Address,
                value => Email.Create(value).Value)
            .IsRequired();

        builder.Property(v => v.Description)
            .IsRequired();

        builder.Property(v => v.Experience)
            .HasConversion(e => e.Year,
                value => Experience.Create(value).Value);

        builder.Property(v => v.PhoneNumber)
            .HasColumnName("phone_number")
            .HasConversion(e => e.Value,
                value => PhoneNumber.Create(value).Value);

        builder.Property(v => v.HelpRequisites)
            .HasConversion(hr => JsonSerializer.Serialize(hr, JsonSerializerOptions.Default),
                value => JsonSerializer.Deserialize<IReadOnlyCollection<HelpRequisite>>(
                    value, JsonSerializerOptions.Default) ?? new List<HelpRequisite>())
            .HasColumnName("help_requisites")
            .HasColumnType("jsonb");

        builder.Property(v => v.SocialNetworks)
            .HasConversion(hr => JsonSerializer.Serialize(hr, JsonSerializerOptions.Default),
                value => JsonSerializer.Deserialize<IReadOnlyCollection<SocialNetwork>>(
                    value, JsonSerializerOptions.Default) ?? new List<SocialNetwork>())
            .HasColumnName("social_networks")
            .HasColumnType("jsonb");
        ;


        builder.Property(v => v.IsDelete)
            .HasColumnName("is_delete");
        builder.Property(v => v.DateDelete)
            .HasColumnName("date_delete");
    }
}