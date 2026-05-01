using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;

/// <summary>
///     VO позиция питомца в списке волонтера
/// </summary>
public class Position : ValueObject
{
    [ExcludeFromCodeCoverage]
    private Position()
    {
    }

    /// <summary>
    ///     ctr
    /// </summary>
    /// <param name="number">Номер позиции</param>
    private Position(int number)
    {
        Number = number;
    }

    /// <summary>
    ///     Номер позиции
    /// </summary>
    public int Number { get; }

    /// <summary>
    ///     Фабричный метод создания позиции
    /// </summary>
    /// <param name="number">Номер позиции</param>
    /// <returns>Position Or Error</returns>
    public static Result<Position, Error> Create(int number)
    {
        if (number <= 0)
            return GeneralErrors.ValueIsInvalid(nameof(number));
        return new Position(number);
    }

    /// <summary>
    ///     Изменение позиции на одну вперед
    /// </summary>
    public Result<Position, Error> Forward()
    {
        return Create(Number + 1);
    }

    /// <summary>
    ///     Изменение позиции на одну назад
    /// </summary>
    public Result<Position, Error> Backward()
    {
        return Create(Number - 1);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }
}