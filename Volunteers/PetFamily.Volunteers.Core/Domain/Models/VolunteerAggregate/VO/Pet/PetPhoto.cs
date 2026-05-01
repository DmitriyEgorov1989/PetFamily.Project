using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;

public class PetPhoto : ValueObject
{
    private const long MAX_SIZE = 5 * 1024 * 1024;

    [ExcludeFromCodeCoverage]
    private PetPhoto()
    {
    }

    private PetPhoto(string pathStorage)
    {
        PathStorage = pathStorage;
        IsMain = false;
    }

    [JsonConstructor]
    private PetPhoto(string pathStorage, bool isMain)
    {
        PathStorage = pathStorage;
        IsMain = isMain;
    }

    public bool IsMain { get; }
    public string PathStorage { get; }

    public static Result<PetPhoto, Error> Create(long size, string pathStorage)
    {
        if (size > MAX_SIZE)
            return GeneralErrors.InvalidSize(nameof(size));
        if (string.IsNullOrWhiteSpace(pathStorage))
            return GeneralErrors.ValueIsInvalid(nameof(pathStorage));

        var path = Guid.NewGuid() + pathStorage;

        return new PetPhoto(path);
    }

    public static Result<PetPhoto, Error> Create(string pathStorage)
    {
        if (string.IsNullOrWhiteSpace(pathStorage))
            return GeneralErrors.ValueIsInvalid(nameof(pathStorage));

        return new PetPhoto(pathStorage);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PathStorage;
    }

    /// <summary>
    ///     Ставит эту фотографию главной,
    ///     если она не является таковой.
    ///     Если же она уже главная, то ничего не происходит.
    /// </summary>
    public PetPhoto MakeMain()
    {
        return new PetPhoto(PathStorage, true);
    }

    /// <summary>
    ///     Убирает статус главной фотографии,
    ///     если она таковой является.
    /// </summary>
    public PetPhoto RemoveMain()
    {
        return new PetPhoto(PathStorage);
    }
}