using CSharpFunctionalExtensions;
using PetFamily.Core.Domain.Models.SpeciesAggregate.VO;
using Primitives;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet
{
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
            if (specieId == default)
                return GeneralErrors.ValueIsRequired(nameof(specieId));
            if (breedId == default)
                return GeneralErrors.ValueIsRequired(nameof(breedId));

            return new PetSpeciesInfo(specieId, breedId);
        }
    }
}