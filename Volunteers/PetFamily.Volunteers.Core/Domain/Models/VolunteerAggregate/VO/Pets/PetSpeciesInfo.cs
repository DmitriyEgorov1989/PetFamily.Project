using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;

namespace PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.VO.Pets;

public class PetSpeciesInfo : ValueObject
{
    private PetSpeciesInfo(SpeciesId specieId, BreedId breedId)
    {
        SpecieId = specieId;
        BreedId = breedId;
    }

    public SpeciesId SpecieId { get; }
    public BreedId BreedId { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SpecieId;
        yield return BreedId;
    }

    public static Result<PetSpeciesInfo, Error> Create(SpeciesId specieId, BreedId breedId)
    {
        return new PetSpeciesInfo(specieId, breedId);
    }
}