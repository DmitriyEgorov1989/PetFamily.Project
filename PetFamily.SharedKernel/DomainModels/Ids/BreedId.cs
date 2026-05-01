using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;

namespace PetFamily.SharedKernel.DomainModels.Ids;

public record BreedId : IComparable<BreedId>
{
    private BreedId(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public int CompareTo(BreedId other)
    {
        if (other is null) return 1;

        return Id.CompareTo(other.Id);
    }

    public static BreedId NewId()
    {
        return new BreedId(Guid.NewGuid());
    }

    public static Result<BreedId, Error> Create(Guid id)
    {
        if (id == default)
            return GeneralErrors.ValueIsInvalid(nameof(id));

        return new BreedId(id);
    }
}