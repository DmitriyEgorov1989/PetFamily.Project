using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Npgsql;
using PetFamily.Core.Abstractions;
using PetFamily.Infrastructure.Options;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;
using PetFamily.Volunteers.Core.Ports;
using PetFamily.Volunteers.Core.Ports.DataBaseForRead;
using PetFamily.Volunteers.Infrastructure.Adapters.MessageQueues;
using PetFamily.Volunteers.Infrastructure.Adapters.Minio;
using PetFamily.Volunteers.Infrastructure.Adapters.Minio.BackgroundServices;
using PetFamily.Volunteers.Infrastructure.Adapters.Postgres.ReadDatabase.Common.TypeHandlers;
using PetFamily.Volunteers.Infrastructure.Adapters.Postgres.ReadDatabase.ConnectionFactory;
using PetFamily.Volunteers.Infrastructure.Adapters.Postgres.ReadDatabase.Repository;
using PetFamily.Volunteers.Infrastructure.Adapters.Postgres.ReadDatabase.Repository.Pets;
using PetFamily.Volunteers.Infrastructure.Adapters.Postgres.WriteDataBase;
using PetFamily.Volunteers.Infrastructure.Adapters.Postgres.WriteDataBase.BackgroundJobs;
using PetFamily.Volunteers.Infrastructure.Adapters.Postgres.WriteDataBase.Repository;
using Quartz;

namespace PetFamily.Volunteers.Infrastructure.DependencyInjection;

public static class InjectVolunteersInfrastructure
{
    public static void AddVolunteersInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataBaseForWrite(configuration)
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
                      ?? throw new ArgumentNullException($"{nameof(MinioOptions)} is null");

        services.AddMinio(cfg =>
        {
            cfg.WithCredentials(options.Login, options.Password)
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

        services.AddDbContext<VolunteersDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<
                IOptions<DataBaseOptions>>().Value;

            if (string.IsNullOrWhiteSpace(dbOptions.ConnectionString))
                throw new InvalidOperationException("Database connection string is missing.");

            options.UseNpgsql(dbOptions.ConnectionString);
            options.UseCamelCaseNamingConvention();
            options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
        });

        services.AddScoped<IVolunteerRepository, VolunteerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
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
                          .Get<DataBaseOptions>()
                      ?? throw new ArgumentNullException($"{nameof(DataBaseOptions.SECTION_NAME)} is null");

        var dataSource = new NpgsqlDataSourceBuilder(options.ConnectionString).Build();
        services.AddSingleton(dataSource);
        services.AddScoped<IReadVollunteersRepository, VolunteersRepository>();
        services.AddScoped<IPetsReadRepository, PetsRepository>();
        services.AddScoped<PetsQueryBuilder>();
        services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory>();
        return services;
    }
}