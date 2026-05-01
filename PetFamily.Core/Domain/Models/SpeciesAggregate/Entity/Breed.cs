using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Core.Domain.Models.SpeciesAggregate.Entity;

/// <summary>
///     Порода животного
/// </summary>
public class Breed : Entity<BreedId>
{
    [ExcludeFromCodeCoverage]
    private Breed()
    {
    }

    /// <summar
    ///     ctr
    /// </summary>
    /// <param name="name">Название породы</param>
    private Breed(BreedId breedId, string name, SpeciesId id)
    {
        Id = breedId;
        Name = name;
        SpeciesId = id;
    }

    /// <summary>
    ///     Название породы
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     VolunteerId типа животного
    /// </summary>
    public SpeciesId SpeciesId { get; private set; }

    public static Result<Breed, Error> Create(BreedId breedId, string name, SpeciesId id)
    {
        if (string.IsNullOrEmpty(name))
            return GeneralErrors.ValueIsInvalid(nameof(name));
        return new Breed(breedId, name, id);
    }
}