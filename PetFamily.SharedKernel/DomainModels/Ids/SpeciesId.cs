using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;

namespace PetFamily.SharedKernel.DomainModels.Ids;

public record SpeciesId : IComparable<SpeciesId>
{
    private SpeciesId(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public int CompareTo(SpeciesId? other)
    {
        if (other is null) return 1;

        return Id.CompareTo(other.Id);
    }

    public static SpeciesId NewId()
    {
        return new SpeciesId(Guid.NewGuid());
    }

    public static Result<SpeciesId, Error> Create(Guid id)
    {
        if (id == default)
            return GeneralErrors.ValueIsInvalid(nameof(id));
        return new SpeciesId(id);
    }
}