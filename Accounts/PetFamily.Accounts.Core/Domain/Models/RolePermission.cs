using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Accounts.Core.Domain.Models;

/// <summary>
/// Сущность, представляющая связь между ролями и разрешениями в системе. 
/// Каждая запись в этой сущности указывает, 
/// что определенная роль имеет определенное разрешение.
/// Это позволяет управлять доступом и правами пользователей на основе их ролей в системе.
/// </summary>
public class RolePermission
{
    [ExcludeFromCodeCoverage]
    private RolePermission() { }
    public RolePermission(Role role, Permission permission)
    {
        Permission = permission;
        Role = role;
        PermissionId = permission.Id;
        RoleId = role.Id;
    }
    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; }

}