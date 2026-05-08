using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Accounts.Core.Domain.Models;

/// <summary>
/// Сущность, представляющая разрешение в системе.
/// </summary>
public class Permission
{
    [ExcludeFromCodeCoverage]
    private Permission() { }
    /// <summary>
    /// ID разрешения, 
    /// который является уникальным идентификатором для каждого разрешения в системе.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Код разрешения, который используется для идентификации и проверки прав доступа.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Навигационное свойство, 
    /// представляющее связь между разрешением и ролями, которые имеют это разрешение.
    /// </summary>
    public List<RolePermission> RolePermissions { get; set; }

    /// <summary>
    /// Метод для создания нового разрешения с заданным кодом.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static Permission Create(string code)
    {
        return new Permission
        {
            Id = Guid.NewGuid(),
            Code = code,
        };
    }
}