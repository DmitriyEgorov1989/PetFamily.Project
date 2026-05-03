using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Core.Domain.Models.AccountAggregate;
using PetFamily.Accounts.Core.Ports;
using PetFamily.Accounts.Infrastructure.Adapters.Jwt;
using PetFamily.Accounts.Infrastructure.Adapters.Postgres;
using PetFamily.Infrastructure.Options;
using System.Text;

namespace PetFamily.Accounts.Infrastructure.DependencyInjection
{
    public static class InjectAccountsInfrastructure
    {
        public static IServiceCollection AddAccountsInfrastructure(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentificationWithJwt(configuration)
                  .AddIdentityService();
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
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey =
                            new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
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
            .AddDefaultTokenProviders(); ;
            return services;
        }
    }
}
