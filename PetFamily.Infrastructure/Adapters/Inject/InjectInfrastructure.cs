using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.ComonDto;
using PetFamily.Core.Ports;
using PetFamily.Infrastructure.Adapters.MessageQueues;
using PetFamily.Infrastructure.Adapters.Minio;
using PetFamily.Infrastructure.Adapters.Minio.BackgroundServices;
using PetFamily.Infrastructure.Adapters.Postgres;
using PetFamily.Infrastructure.Adapters.Postgres.BackgroundJobs;
using PetFamily.Infrastructure.Adapters.Postgres.Repository;
using PetFamily.Infrastructure.Options;
using Quartz;

namespace PetFamily.Infrastructure.Adapters.Inject
{
    public static class InjectInfrastructure
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IVolunteerRepository, VolonteerRepository>();
            services.AddScoped<IFileStorageProvider, MinioService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddHostedService<FilesCleanupJob>();
            services.AddSingleton<IMessageQueueService<IEnumerable<PetPhotoDto>>, FilesCleanupMessageQueue>();

            services.Configure<QuartzJobOptions>(
                configuration.GetSection(QuartzJobOptions.SECTION_NAME));

            AddMinioService(services, configuration);
        }

        /// <summary>
        /// Регистрация минио сервиса
        /// </summary>
        /// <param name="services">интерфейс DI</param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException">Error if not found options</exception>
        private static void AddMinioService(this IServiceCollection services, IConfiguration configuration)
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
        }

        /// <summary>
        /// Регистраци Quartz job сервиса
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

            services.AddQuartzHostedService(q =>
            {
                q.WaitForJobsToComplete = true;
            });

            return services;
        }
    }
}