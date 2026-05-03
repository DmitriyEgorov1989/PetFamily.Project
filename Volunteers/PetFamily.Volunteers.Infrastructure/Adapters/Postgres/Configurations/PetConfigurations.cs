using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.Entity.Pet;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.VO.Pets;
using System.Text.Json;

namespace PetFamily.Infrastructure.Adapters.Postgres.WriteDataBase.Configurations
{
    public class PetsConfiguration : IEntityTypeConfiguration<Pet>
    {
        public void Configure(EntityTypeBuilder<Pet> builder)
        {
            builder.ToTable("pets");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .IsRequired()
                .HasColumnName("pet_id")
                .ValueGeneratedNever()
                .HasConversion(p => p.Id,
                 value => PetId.Create(value).Value);

            builder.Property(p => p.Name)
                   .IsRequired();

            builder.Property(p => p.Description)
                   .IsRequired();

            builder.OwnsOne(p => p.SpeciesInfo, pb =>
            {
                pb.Property(si => si.SpecieId)
                  .HasColumnName("species_id")
                  .HasConversion(i => i.Id,
                  value => SpeciesId.Create(value).Value)
                  .IsRequired();

                pb.Property(si => si.BreedId)
                  .HasColumnName("breed_id")
                  .HasConversion(i => i.Id,
                  value => BreedId.Create(value).Value)
                  .IsRequired();
            });

            builder.Property(p => p.Color)
                    .HasConversion(c => c.Name,
                    value => Color.Create(value).Value)
                   .IsRequired();

            builder.Property(p => p.HealthInfo)
                   .HasColumnName("health_info")
                   .HasConversion(h => h.Description,
                   value => HealthInfo.Create(value).Value)
                   .IsRequired();

            builder.ComplexProperty(p => p.Address, ab =>
            {
                ab.IsRequired();

                ab.Property(a => a.City)
                  .HasMaxLength(Address.MAX_LENGTH_NAME)
                  .HasColumnName("city")
                  .IsRequired();

                ab.Property(a => a.Region)
                 .HasMaxLength(Address.MAX_LENGTH_NAME)
                 .HasColumnName("region")
                 .IsRequired();

                ab.Property(a => a.House)
                  .HasMaxLength(Address.MAX_LENGTH_NAME)
                 .HasColumnName("house")
                 .IsRequired();
            });

            builder.Property(p => p.PetHelpRequisites)
                .HasConversion(hr => JsonSerializer.Serialize(hr, JsonSerializerOptions.Default),
                    value => JsonSerializer.Deserialize<List<HelpRequisite>>(
                        value, JsonSerializerOptions.Default) ?? new List<HelpRequisite>())
                .HasColumnName("help_requisites")
                .HasColumnType("jsonb");

            builder.Ignore(p => p.Photos);

            builder.Property<List<PetPhoto>>("_photos")
                .HasColumnName("photos")
                .HasColumnType("jsonb")
                .HasConversion(
                    photos => JsonSerializer.Serialize(photos, JsonSerializerOptions.Default),
                    value => JsonSerializer.Deserialize<List<PetPhoto>>(value, JsonSerializerOptions.Default)
                             ?? new List<PetPhoto>());

            builder.Property(p => p.Weight)
                .IsRequired();

            builder.Property(p => p.Height)
                    .IsRequired();

            builder.Property(p => p.BirthDate)
                    .HasColumnName("birth_date")
                    .IsRequired();

            builder.Property(p => p.IsVaccined)
                    .HasColumnName("is_vaccined")
                    .IsRequired();

            builder.Property(p => p.PhoneNumber)
                    .HasColumnName("phone_number")
                   .HasConversion(pn => pn.Value,
                      value => PhoneNumber.Create(value).Value)
                   .IsRequired();

            builder.Property(p => p.IsSterilized)
                   .HasColumnName("is_sterilized")
                    .IsRequired();

            builder.Property(p => p.PetHelpStatus)
                .HasColumnName("pet_help_status")
                .HasConversion(hs => Convert.ToInt32(hs),
                value => Pet.ToHelpStatus(value))
                .IsRequired();

            builder.Property(p => p.CreatedUtc)
                .HasColumnName("created_otc")
                .IsRequired();

            builder.Property(p => p.VolunteerId)
                    .HasColumnName("volunteer_id")
                    .HasConversion(i => i.Id, value =>
                            VolunteerId.Create(value).Value)
                    .IsRequired();

            builder.Property(p => p.Position)
                .HasColumnName("position")
                .HasConversion(p => p.Number, value =>
                    Position.Create(value).Value)
                .IsRequired();

            builder.HasOne(p => p.VolunteerNavigation)
                   .WithMany(v => v.Pets)
                   .HasForeignKey(v => v.VolunteerId)
                   .IsRequired()
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.IsDelete)
                .HasColumnName("is_delete");
            builder.Property(p => p.DateDelete)
                .HasColumnName("date_delete");
        }
    }
}