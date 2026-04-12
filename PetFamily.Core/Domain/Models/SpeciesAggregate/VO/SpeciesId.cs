using CSharpFunctionalExtensions;
using Primitives;

namespace PetFamily.Core.Domain.Models.SpeciesAggregate.VO
{
    public record SpeciesId:IComparable<SpeciesId>
    {
        private SpeciesId(Guid id)
        {
            Id=id;
        }
        public Guid Id { get; }

        public static Guid NewId() => Guid.NewGuid();

        public static Result<SpeciesId, Error> Create(Guid id)
        {
            if (id == default)
                return GeneralErrors.ValueIsInvalid(nameof(id));
            return new SpeciesId(id);
        }

        public int CompareTo(SpeciesId? other)
        {
            if (other is null) return 1;

            return Id.CompareTo(other.Id);
        }
    }
}