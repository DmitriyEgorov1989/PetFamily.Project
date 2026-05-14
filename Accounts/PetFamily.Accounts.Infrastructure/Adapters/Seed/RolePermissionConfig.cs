namespace PetFamily.Accounts.Infrastructure.Adapters.Seed;

public record class RolePermissionConfig(
    Dictionary<string, string[]> Roles,
    Dictionary<string, string[]> Permissions);