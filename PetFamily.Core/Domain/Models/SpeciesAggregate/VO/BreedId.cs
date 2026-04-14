using CSharpFunctionalExtensions;
using Primitives;

namespace PetFamily.Core.Domain.Models.SpeciesAggregate.VO
{
    public record BreedId : IComparable<BreedId>
    {
        private BreedId(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; }

        public static BreedId NewId() => new(Guid.NewGuid());

        public static Result<BreedId, Error> Create(Guid id)
        {
            if (id == default)
                return GeneralErrors.ValueIsInvalid(nameof(id));

            return new BreedId(id);
        }

        public int CompareTo(BreedId other)
        {
            if (other is null) return 1;

            return Id.CompareTo(other.Id);
        }
    }
}