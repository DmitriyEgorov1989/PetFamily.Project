using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.Accounts.Infrastructure.Adapters.Postgres;
using Serilog;

namespace PetFamily.Accounts.Infrastructure.Adapters.Seed;

public static class AccountsSeederExtension
{
    public static async Task SeedAsync(this AccountDbContext context,
        RoleManager<Role> roleManager, RolePermissionConfig seedData)
    {
        var permissionsToAdd =
            seedData.Permissions
                .SelectMany(permissionGroup => permissionGroup.Value);

        var permissions = await EnsurePermissionsExistAsync(context, permissionsToAdd);
        await context.Permissions.AddRangeAsync(permissions);

        await EnsureRoleAndPermissionsAsync(context, roleManager, seedData, permissions);
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
                    throw new Exception("Error create new role"); ;
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
}

public class Seeder : ISeeder
{
    private readonly AccountDbContext _dbContext;
    private readonly ILogger _logger;
    private readonly SeederOptions _options;
    private readonly RoleManager<Role> _roleManager;

    public Seeder(
        IOptions<SeederOptions> options,
        ILogger logger,
        RoleManager<Role> roleManager, AccountDbContext dbContext)
    {
        _options = options.Value;
        _logger = logger;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        try
        {
            var seedData = await GetSeedData();
            _logger.Information("Starting seeding process...");
            await _dbContext.SeedAsync(_roleManager, seedData);
            _logger.Information("Seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred during seeding.");
        }
    }

    private async Task<RolePermissionConfig> GetSeedData()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory,
            _options.JsonFilePath.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Seed file not found: {filePath}");

        var json =
            await File.ReadAllTextAsync(
                filePath);

        var seedData = JsonConvert.DeserializeObject<RolePermissionConfig>(json)
                       ?? throw new Exception("Error read json at seeding");
        return seedData;
    }
}

public class SeederOptions
{
    public const string SECTION_NAME = "Seeder";
    public string JsonFilePath { get; set; } = string.Empty;
}

public record class RolePermissionConfig(
    Dictionary<string, string[]> Roles,
    Dictionary<string, string[]> Permissions);