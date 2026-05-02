using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate.VO;

/// <summary>
///     Value Object - социальная сеть,содержащие название соц.сети и ссылку
/// </summary>
public sealed class SocialNetwork : ValueObject
{
    [ExcludeFromCodeCoverage]
    public SocialNetwork()
    {
    }

    private SocialNetwork(string name, string link)
    {
        Name = name;
        Link = link;
    }

    /// <summary>
    ///     Название социальной сети
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     ссылка на соц.сеть
    /// </summary>
    public string Link { get; }

    public static Result<SocialNetwork, Error> Create(string name, string link)
    {
        if (string.IsNullOrEmpty(name))
            return GeneralErrors.ValueIsInvalid(nameof(name));
        if (string.IsNullOrEmpty(link))
            return GeneralErrors.ValueIsInvalid(nameof(link));
        return new SocialNetwork(name, link);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Link;
    }
}