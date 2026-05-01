using Microsoft.Extensions.Options;
using PetFamily.Core.Ports;
using PetFamily.Infrastructure.Options;
using Quartz;

namespace PetFamily.Volunteers.Infrastructure.Adapters.Postgres.WriteDataBase.BackgroundJobs;

public class HardDeleteVolunteerByTimeJob : IJob
{
    private readonly ILogger _logger;
    private readonly QuartzJobOptions _options;
    private readonly IVolunteerRepository _volunteerRepository;

    public HardDeleteVolunteerByTimeJob(
        IVolunteerRepository volunteerRepository
        , ILogger logger,
        IOptions<QuartzJobOptions> options)
    {
        _volunteerRepository = volunteerRepository;
        _logger = logger;
        _options = options.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.Information("Start job Hard delete volunteers ");

        context.CancellationToken.ThrowIfCancellationRequested();

        var volunteers = await _volunteerRepository
            .GetAllDeleteAsync(context.CancellationToken);
        var volunteersForHardDelete =
            volunteers.Where(v => v.DateDelete <= DateTime.UtcNow.AddDays(-_options.StorageTimeDays))
                .ToList();

        if (!volunteersForHardDelete.Any())
        {
            _logger.Information("There are no volunteers to remove it ");
            return;
        }

        var deleteTasks = volunteersForHardDelete.Select(v =>
        {
            _logger.Information("Volunteer with id {v.VolunteerId} delete", v.Id);
            return _volunteerRepository.DeleteAsync(v.Id, context.CancellationToken);
        }).ToList();
        await Task.WhenAll(deleteTasks);

        _logger.Information(
            "Hard delete completed. Removed volunteers count: {Count}",
            volunteersForHardDelete.Count);
    }
}