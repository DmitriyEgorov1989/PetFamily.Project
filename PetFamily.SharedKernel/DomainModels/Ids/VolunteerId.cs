using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;

namespace PetFamily.SharedKernel.DomainModels.Ids;

public record VolunteerId : IComparable<VolunteerId>
{
    private VolunteerId(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public int CompareTo(VolunteerId? other)
    {
        if (other is null) return 1;

        return Id.CompareTo(other.Id);
    }

    public static VolunteerId NewId()
    {
        return new VolunteerId(Guid.NewGuid());
    }

    public static Result<VolunteerId, Error> Create(Guid id)
    {
        if (id == default)
            return GeneralErrors.ValueIsInvalid(nameof(id));
        return new VolunteerId(id);
    }

    public static implicit operator Guid(VolunteerId id)
    {
        return id.Id;
    }
}