using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.Accounts.Core.Domain.Models.Accounts;
using PetFamily.Accounts.Infrastructure.Adapters.Postgres;
using PetFamily.Core.Options;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.DomainModels.VO;

namespace PetFamily.Accounts.Infrastructure.Adapters.Seed;

public static class AccountsSeederExtension
{
    public static async Task SeedAsync(this AccountDbContext context,
        RoleManager<Role> roleManager,
        RolePermissionConfig seedData,
        IOptions<AdminOptions> options,
        UserManager<User> userManager)
    {
        var permissionsToAdd =
            seedData.Permissions
                .SelectMany(permissionGroup => permissionGroup.Value);

        var permissions = await EnsurePermissionsExistAsync(context, permissionsToAdd);

        if (permissions.Any())
        {
            await context.Permissions.AddRangeAsync(permissions);
        }
        permissions = context.Permissions.ToList();
        await EnsureRoleAndPermissionsAsync(context, roleManager, seedData, permissions);
        await EnsureAdminAccountAsync(context, options, userManager);
        await context.SaveChangesAsync();
    }

    private static async Task<List<Permission>> EnsurePermissionsExistAsync(
        AccountDbContext context,
        IEnumerable<string> permissionCodes)
    {
        var listPermissions = new List<Permission>();
        foreach (var permissionCode in permissionCodes)
        {
            var isPermissionExist =
                context.Permissions.Any(permission => permissionCode == permission.Code);

            if (isPermissionExist)
                continue;

            listPermissions.Add(Permission.Create(permissionCode));
        }

        await context.Permissions.AddRangeAsync(listPermissions);
        return listPermissions;
    }

    private static async Task EnsureRoleAndPermissionsAsync(
        AccountDbContext context,
        RoleManager<Role> roleManager,
        RolePermissionConfig seedData,
        List<Permission> permissions)
    {
        var roles = context.Roles
            .Include(r => r.RolePermissions).ThenInclude(rolePermission => rolePermission.Permission)
            .ToList();

        foreach (var roleData in seedData.Roles)
        {
            var role = roles.FirstOrDefault(r => r.Name == roleData.Key);

            if (role is null)
            {
                role = new Role { Name = roleData.Key };

                var resultCreateRole = await roleManager
                    .CreateAsync(role);
                if (!resultCreateRole.Succeeded)
                    throw new Exception("Error create new role");
                ;
            }

            foreach (var permission in roleData.Value)
            {
                var permissionToAdd = permissions.FirstOrDefault(p => p.Code == permission);

                if (permissionToAdd is null)
                    throw new Exception("Error seeding permissions");

                var isExistPermission = role.RolePermissions
                    .Any(p => p.Permission.Code == permission);

                if (isExistPermission)
                    continue;

                context.RolePermissions.Add(new RolePermission(role, permissionToAdd));
            }
        }
    }

    private static async Task EnsureAdminAccountAsync(
        AccountDbContext context, IOptions<AdminOptions> options, UserManager<User> userManager)
    {
        var adminOptions = options.Value;

        var role =
            await context.Roles.FirstOrDefaultAsync(r => r.Name == RolesName.Admin);
        if (role is null)
            throw new ArgumentNullException(nameof(RolesName.Admin));

        var isAdminExist =
            await context.Users.AnyAsync(u => u.RoleId == role.Id);
        if (isAdminExist)
            return;

        var fullName =
            FullName.Create(adminOptions.FirstName, adminOptions.MiddleName, adminOptions.LastName).Value;
        var user =
            User.Create(
                Guid.NewGuid(),
                fullName,
                adminOptions.Email,
                adminOptions.UserName,
                adminOptions.PhoneNumber).Value;

        var accountAdmin = Admin.Create(user.Id);

        user.AddRole(role, accountAdmin, null, null);

        await userManager.CreateAsync(user, adminOptions.Password);
    }
}