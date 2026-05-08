using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.Accounts.Core.Ports;
using PetFamily.Accounts.Infrastructure.Adapters.Jwt;
using PetFamily.Accounts.Infrastructure.Adapters.Postgres;
using PetFamily.Accounts.Infrastructure.Adapters.Seed;
using PetFamily.Core.Abstractions;
using PetFamily.Infrastructure.Options;
using System.Text;

namespace PetFamily.Accounts.Infrastructure.DependencyInjection;

public static class InjectAccountsInfrastructure
{
    public static IServiceCollection AddAccountsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentificationWithJwt(configuration)
            .AddIdentityService()
            .AddDataBaseForWrite(configuration)
            .AddSeeder(configuration);

        return services;
    }

    private static IServiceCollection AddSeeder(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SeederOptions>(
            configuration.GetSection(SeederOptions.SECTION_NAME));
        services.AddScoped<ISeeder, Seeder>();
        return services;
    }

    private static IServiceCollection AddDataBaseForWrite
        (this IServiceCollection services, IConfiguration configure)
    {
        services.Configure<DataBaseOptions>(
            configure.GetSection(DataBaseOptions.SECTION_NAME));

        services.AddDbContext<AccountDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<
                IOptions<DataBaseOptions>>().Value;

            if (string.IsNullOrWhiteSpace(dbOptions.ConnectionString))
                throw new InvalidOperationException("Database connection string is missing.");

            options.UseNpgsql(dbOptions.ConnectionString);
            options.UseCamelCaseNamingConvention();
            options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    private static IServiceCollection AddAuthentificationWithJwt(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SECTION_NAME));
        services.AddScoped<ITokenProvider, JwtTokenProvider>();

        var jwtOptions = configuration.GetSection(JwtOptions.SECTION_NAME)
            .Get<JwtOptions>();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });
        return services;
    }

    private static IServiceCollection AddIdentityService(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AccountDbContext>()
            .AddDefaultTokenProviders();
        ;
        return services;
    }
}