using CSharpFunctionalExtensions;
using Primitives;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate.VO
{
    public record VolunteerId : IComparable<VolunteerId>
    {
        private VolunteerId(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; }

        public static VolunteerId NewId() => new(Guid.NewGuid());

        public static Result<VolunteerId, Error> Create(Guid id)
        {
            if (id == default)
                return GeneralErrors.ValueIsInvalid(nameof(id));
            return new VolunteerId(id);
        }

        public int CompareTo(VolunteerId? other)
        {
            if (other is null) return 1;

            return Id.CompareTo(other.Id);
        }

        public static implicit operator Guid(VolunteerId id) => id.Id;
    }
}