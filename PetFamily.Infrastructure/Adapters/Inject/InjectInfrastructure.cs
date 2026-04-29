using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using Npgsql;
using PetFamily.Core.Application.UseCases.CommonDto;
using PetFamily.Core.Domain.Models.AccountAggregate;
using PetFamily.Core.Ports;
using PetFamily.Core.Ports.DataBaseForRead;
using PetFamily.Infrastructure.Adapters.Jwt;
using PetFamily.Infrastructure.Adapters.MessageQueues;
using PetFamily.Infrastructure.Adapters.Minio;
using PetFamily.Infrastructure.Adapters.Minio.BackgroundServices;
using PetFamily.Infrastructure.Adapters.Postgres.ReadDatabase.Common.TypeHandlers;
using PetFamily.Infrastructure.Adapters.Postgres.ReadDatabase.ConnectionFactory;
using PetFamily.Infrastructure.Adapters.Postgres.ReadDatabase.Repository;
using PetFamily.Infrastructure.Adapters.Postgres.ReadDatabase.Repository.Pets;
using PetFamily.Infrastructure.Adapters.Postgres.WriteDataBase;
using PetFamily.Infrastructure.Adapters.Postgres.WriteDataBase.BackgroundJobs;
using PetFamily.Infrastructure.Adapters.Postgres.WriteDataBase.Repository;
using PetFamily.Infrastructure.Options;
using Quartz;

namespace PetFamily.Infrastructure.Adapters.Inject;

public static class InjectInfrastructure
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataBaseForWrite(configuration)
            .AddIdentityService()
            .AddAuthentificationWithJwt(configuration)
            .AddDataBaseForRead(configuration)
            .AddMinioService(configuration)
            .AddQuartzJob(configuration);

        DapperTypeHandlerRegistration.AddDapperTypeHandlers();

        services.AddHostedService<FilesCleanupJob>();
        services.AddSingleton<IMessageQueueService<IEnumerable<PetPhotoDto>>, FilesCleanupMessageQueue>();
    }

    /// <summary>
    ///     Регистрация минио сервиса
    /// </summary>
    /// <param name="services">Интерфейс DI</param>
    /// <param name="configuration">Конфигурация приложения</param>
    /// <exception cref="ArgumentNullException">Error if not found options</exception>
    private static IServiceCollection AddMinioService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(
            configuration.GetSection(MinioOptions.SECTION_NAME));

        var options = configuration.GetSection(MinioOptions.SECTION_NAME)
                          .Get<MinioOptions>()
                      ?? throw new ArgumentNullException("Error get options minio");

        services.AddMinio(configuration =>
        {
            configuration.WithCredentials(options.Login, options.Password)
                .WithSSL(false)
                .WithEndpoint(options.ConnectionString)
                .Build();
        });
        services.AddScoped<IFileStorageProvider, MinioService>();
        return services;
    }

    /// <summary>
    ///     Регистраци Quartz job сервиса
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private static IServiceCollection AddQuartzJob(
        this IServiceCollection services
        , IConfiguration configure)
    {
        var options = configure.GetSection(QuartzJobOptions.SECTION_NAME).Get<QuartzJobOptions>()
                      ?? throw new ArgumentNullException(nameof(configure));

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey(options.HardDeleteVolunteerIdentity);

            q.AddJob<HardDeleteVolunteerByTimeJob>(opts =>
                opts.WithIdentity(jobKey));

            q.AddTrigger(opts =>
            {
                opts.ForJob(jobKey)
                    .WithIdentity(options.HardDeleteVolunteerTrigger)
                    .WithCronSchedule(options.CronShedule);
            });
        });

        services.AddQuartzHostedService(q => { q.WaitForJobsToComplete = true; });

        return services;
    }

    /// <summary>
    ///     Регистрация сервисов базы данных для записи
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    private static IServiceCollection AddDataBaseForWrite
        (this IServiceCollection services, IConfiguration configure)
    {
        services.Configure<DataBaseOptions>(
            configure.GetSection(DataBaseOptions.SECTION_NAME));

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<
                Microsoft.Extensions.Options.IOptions<DataBaseOptions>>().Value;

            if (string.IsNullOrWhiteSpace(dbOptions.ConnectionString))
                throw new InvalidOperationException("Database connection string is missing.");

            options.UseNpgsql(dbOptions.ConnectionString);
            options.UseCamelCaseNamingConvention();
            options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
        });

        services.AddScoped<IVolunteerRepository, VolunteerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddDbContext<AccountDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<
                Microsoft.Extensions.Options.IOptions<DataBaseOptions>>().Value;

            if (string.IsNullOrWhiteSpace(dbOptions.ConnectionString))
                throw new InvalidOperationException("Database connection string is missing.");

            options.UseNpgsql(dbOptions.ConnectionString);
            options.UseCamelCaseNamingConvention();
            options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
        });

        return services;
    }

    /// <summary>
    ///     Регистрация бд ждя чтения
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    private static IServiceCollection AddDataBaseForRead
        (this IServiceCollection services, IConfiguration configure)
    {
        var options = configure.GetSection(DataBaseOptions.SECTION_NAME)
            .Get<DataBaseOptions>();

        var dataSource = new NpgsqlDataSourceBuilder(options.ConnectionString).Build();
        services.AddSingleton(dataSource);
        services.AddScoped<IReadVollunteersRepository, VolunteersRepository>();
        services.AddScoped<IPetsReadRepository, PetsRepository>();
        services.AddScoped<PetsQueryBuilder>();
        services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory>();
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
                            System.Text.Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
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