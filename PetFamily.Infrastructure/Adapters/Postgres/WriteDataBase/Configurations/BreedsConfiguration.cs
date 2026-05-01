using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Domain.Models.Species;
using PetFamily.Core.Domain.Models.SpeciesAggregate.Entity;
using PetFamily.SharedKernel.DomainModels.Ids;

namespace PetFamily.Infrastructure.Adapters.Postgres.WriteDataBase.Configurations;

public class BreedsConfiguration : IEntityTypeConfiguration<Breed>
{
    public void Configure(EntityTypeBuilder<Breed> builder)
    {
        builder.ToTable("breeds");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasConversion(b => b.Id,
                value => BreedId.Create(value).Value)
            .HasColumnName("breed_id")
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(b => b.Name)
            .IsRequired();

        builder.Property(b => b.SpeciesId)
            .HasColumnName("species_id")
            .HasConversion(s => s.Id,
                value => SpeciesId.Create(value).Value)
            .IsRequired();

        builder.HasOne<Species>()
            .WithMany(s => s.Breeds)
            .HasForeignKey(b => b.SpeciesId)
            .HasPrincipalKey(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}