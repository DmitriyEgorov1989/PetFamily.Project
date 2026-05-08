using Microsoft.AspNetCore.Identity;

namespace PetFamily.Accounts.Core.Domain.Models;
/// <summary>
/// Сущность, представляющая роль в системе, 
/// которая наследуется от IdentityRole с типом ключа Guid.
/// </summary>
public class Role : IdentityRole<Guid>
{
    /// <summary>
    /// Навигационное свойство,
    /// представляющее список разрешений, связанных с данной ролью.
    /// </summary>
    public List<RolePermission> RolePermissions { get; set; } = [];
}