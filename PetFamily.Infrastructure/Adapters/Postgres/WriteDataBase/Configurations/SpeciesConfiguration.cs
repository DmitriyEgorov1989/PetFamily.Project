using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Domain.Models.Species;
using PetFamily.SharedKernel.DomainModels.Ids;

namespace PetFamily.Infrastructure.Adapters.Postgres.WriteDataBase.Configurations;

public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
{
    public void Configure(EntityTypeBuilder<Species> builder)
    {
        builder.ToTable("species");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .IsRequired()
            .HasColumnName("species_id")
            .ValueGeneratedNever()
            .HasConversion(i => i.Id,
                value => SpeciesId.Create(value).Value);

        builder.Property(s => s.Name)
            .IsRequired();
    }
}