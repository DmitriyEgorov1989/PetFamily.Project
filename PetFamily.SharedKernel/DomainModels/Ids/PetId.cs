using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;

namespace PetFamily.SharedKernel.DomainModels.Ids;

public record PetId : IComparable<PetId>
{
    private PetId(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }

    public int CompareTo(PetId? other)
    {
        if (other is null) return 1;

        return Id.CompareTo(other.Id);
    }

    public static PetId NewId()
    {
        return Create(Guid.NewGuid()).Value;
    }

    public static Result<PetId, Error> Create(Guid id)
    {
        if (id == default)
            return GeneralErrors.ValueIsInvalid(nameof(id));
        return new PetId(id);
    }

    public static implicit operator Guid(PetId id)
    {
        return id.Id;
    }
}