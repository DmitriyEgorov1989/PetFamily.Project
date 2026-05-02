using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.DomainModels;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;
using PetFamily.Species.Core.Domain.Models.SpeciesAggregate.Entity;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Species.Core.Domain.Models.SpeciesAggregate;

/// <summary>
///     Аггрегат вид животного
/// </summary>
public class Species : Aggregate<SpeciesId>
{
    private readonly List<Breed> _breeds = new();

    [ExcludeFromCodeCoverage]
    private Species()
    {
    }

    private Species(SpeciesId id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    ///     Название вида(например кошка, собака)
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     список пород у данного вида
    /// </summary>
    public IReadOnlyCollection<Breed> Breeds => _breeds;

    public static Result<Species, Error> Create(SpeciesId speciesId, string name, List<Breed>? breeds = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return GeneralErrors.ValueIsInvalid(nameof(name));


        return new Species(speciesId, name);
    }

    public UnitResult<Error> Add(Breed breed)
    {
        if (breed == null)
            return UnitResult.Failure(GeneralErrors.ValueIsInvalid(nameof(breed)));

        _breeds.Add(breed);
        return UnitResult.Success<Error>();
    }
}