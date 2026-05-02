using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.SharedKernel.DomainModels.Ids;

namespace PetFamily.Species.Infrastructure.Adapters.Postgres.WriteDataBase.Configurations;

public class SpeciesConfiguration : IEntityTypeConfiguration<Core.Domain.Models.SpeciesAggregate.Species>
{
    public void Configure(EntityTypeBuilder<Core.Domain.Models.SpeciesAggregate.Species> builder)
    {
        builder.ToTable("species", "species");

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