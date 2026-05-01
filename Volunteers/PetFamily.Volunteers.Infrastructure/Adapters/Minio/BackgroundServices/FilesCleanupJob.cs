using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetFamily.Core.Application.UseCases.CommonDto;
using PetFamily.Core.Ports;

namespace PetFamily.Volunteers.Infrastructure.Adapters.Minio.BackgroundServices;

public class FilesCleanupJob : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IMessageQueueService<IEnumerable<PetPhotoDto>> _queueService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public FilesCleanupJob(
        ILogger logger,
        IMessageQueueService<IEnumerable<PetPhotoDto>> queueService,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _queueService = queueService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Information("FilesCleanupJob started");
        while (!stoppingToken.IsCancellationRequested)
        {
            var queue = await _queueService.ReadAsync(stoppingToken);
            using var scope = _serviceScopeFactory.CreateScope();
            var fileStorageProvider = scope.ServiceProvider.GetRequiredService<IFileStorageProvider>();
            foreach (var message in queue)
                await fileStorageProvider.DeleteFileAsync(message.PathStorage, stoppingToken);

            _logger.Information("All files deleted");
        }

        _logger.Information("FilesCleanupJob started finished");
    }
}