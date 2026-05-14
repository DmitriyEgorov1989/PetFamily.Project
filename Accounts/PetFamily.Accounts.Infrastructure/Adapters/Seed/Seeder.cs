using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.Accounts.Infrastructure.Adapters.Postgres;
using PetFamily.Core.Options;
using Serilog;

namespace PetFamily.Accounts.Infrastructure.Adapters.Seed;

public class Seeder : ISeeder
{
    private readonly AccountDbContext _dbContext;
    private readonly ILogger _logger;
    private readonly SeederOptions _seederOptions;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IOptions<AdminOptions> _adminOptions;

    public Seeder(
        IOptions<SeederOptions> seederOptions,
        ILogger logger,
        RoleManager<Role> roleManager,
        AccountDbContext dbContext,
        UserManager<User> userManager,
        IOptions<AdminOptions> adminOptions)
    {
        _logger = logger;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _seederOptions = seederOptions.Value;
        _userManager = userManager;
        _adminOptions = adminOptions;
    }

    public async Task SeedAsync()
    {
        try
        {
            var seedData = await GetSeedData();
            _logger.Information("Starting seeding process...");
            await _dbContext.SeedAsync(_roleManager, seedData, _adminOptions, _userManager);
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
            _seederOptions.JsonFilePath.Replace('/', Path.DirectorySeparatorChar));
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