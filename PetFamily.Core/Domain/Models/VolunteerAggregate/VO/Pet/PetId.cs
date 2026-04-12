using CSharpFunctionalExtensions;
using Primitives;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet
{
    public record PetId : IComparable<PetId>
    {
        private PetId(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; set; }

        public static PetId NewId() => Create(Guid.NewGuid()).Value;

        public static Result<PetId, Error> Create(Guid id)
        {
            if (id == default)
                return GeneralErrors.ValueIsInvalid(nameof(id));
            return new PetId(id);
        }

        public int CompareTo(PetId? other)
        {
            if (other is null) return 1;

            return Id.CompareTo(other.Id);
        }

        public static implicit operator Guid(PetId id) => id.Id;
    }
}