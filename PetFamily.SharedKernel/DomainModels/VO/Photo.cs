using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PetFamily.SharedKernel.DomainModels.VO;

public class Photo : ValueObject
{
    private const long MAX_SIZE = 5 * 1024 * 1024;

    [ExcludeFromCodeCoverage]
    private Photo()
    {
    }

    private Photo(string pathStorage)
    {
        PathStorage = pathStorage;
        IsMain = false;
    }

    [JsonConstructor]
    private Photo(string pathStorage, bool isMain)
    {
        PathStorage = pathStorage;
        IsMain = isMain;
    }

    public bool IsMain { get; }
    public string PathStorage { get; }

    public static Result<Photo, Error> Create(long size, string pathStorage)
    {
        if (size > MAX_SIZE)
            return GeneralErrors.InvalidSize(nameof(size));
        if (string.IsNullOrWhiteSpace(pathStorage))
            return GeneralErrors.ValueIsInvalid(nameof(pathStorage));

        var path = Guid.NewGuid() + pathStorage;

        return new Photo(path);
    }

    public static Result<Photo, Error> Create(string pathStorage)
    {
        if (string.IsNullOrWhiteSpace(pathStorage))
            return GeneralErrors.ValueIsInvalid(nameof(pathStorage));

        return new Photo(pathStorage);
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
    public Photo MakeMain()
    {
        return new Photo(PathStorage, true);
    }

    /// <summary>
    ///     Убирает статус главной фотографии,
    ///     если она таковой является.
    /// </summary>
    public Photo RemoveMain()
    {
        return new Photo(PathStorage);
    }
}