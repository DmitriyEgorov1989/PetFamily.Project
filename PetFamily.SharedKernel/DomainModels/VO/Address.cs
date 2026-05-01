using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.SharedKernel.DomainModels.VO;

/// <summary>
///     Value Object- аддресс
/// </summary>
public class Address : ValueObject
{
    public const int MAX_LENGTH_NAME = 50;

    [ExcludeFromCodeCoverage]
    public Address()
    {
    }

    private Address(string city, string region, string house)
    {
        City = city;
        Region = region;
        House = house;
    }

    /// <summary>
    ///     Город
    /// </summary>
    public string City { get; }

    /// <summary>
    ///     Улица
    /// </summary>
    public string Region { get; }

    /// <summary>
    ///     Дом
    /// </summary>
    public string House { get; }

    public static Result<Address, Error> Create(string city, string region, string house)
    {
        if (string.IsNullOrWhiteSpace(city))
            return GeneralErrors.ValueIsInvalid(nameof(city));
        if (string.IsNullOrWhiteSpace(region))
            return GeneralErrors.ValueIsInvalid(nameof(region));
        if (string.IsNullOrWhiteSpace(house))
            return GeneralErrors.ValueIsInvalid(nameof(house));

        return new Address(city, region, house);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return City;
        yield return Region;
        yield return House;
    }
}