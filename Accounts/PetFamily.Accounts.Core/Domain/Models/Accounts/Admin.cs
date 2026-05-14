using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Accounts.Core.Domain.Models.Accounts;

/// <summary>
///     Сущность,
///     представляющая администратора в системе.
/// </summary>
public class Admin
{
    [ExcludeFromCodeCoverage]
    private Admin()
    {
    }
    public Guid Id { get; set; }
    /// <summary>
    /// UserId администратора, 
    /// который является уникальным идентификатором для каждого администратора в системе.
    /// </summary>
    public Guid UserId { get; set; }
    public User User { get; set; }

    public static Admin Create(Guid userId)
    {
        return new Admin
        {
            UserId = userId
        };
    }
}