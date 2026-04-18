using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Core.Application.UseCases.ComonDto;
using PetFamily.Core.Ports.DataBaseForRead;
using Primitives;
using Serilog;

namespace PetFamily.Infrastructure.Adapters.Postgres.ReadDatabase.Repository;

public class ReadRepository : IReadRepository
{
    private const string SQL =
        """
        SELECT 
            v.volunteer_id as VolunteerId,
            v.first_name as FirstName,
            v.middle_name as MiddleName,
            v.last_name as LastName,
            v.email,
            v.description,
            v.experience,
            v.phone_number as PhoneNumber,
            v.help_requisites as HelpRequisites,
            v.social_networks as SocialNetworks from volunteers as v
        where v.is_delete = false;
        """;

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger _logger;

    public ReadRepository(IDbConnectionFactory connectionFactory, ILogger logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<VolunteerDto>, Error>> GetAllVolunteersAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

            var command = new CommandDefinition(SQL, cancellationToken);

            var volunteers =
                await connection.QueryAsync<VolunteerDto>(command);
            if (volunteers is null || !volunteers.Any())
            {
                _logger.Information("Volunteers not found");
                return GeneralErrors.NotFound();
            }

            return Result.Success<IEnumerable<VolunteerDto>, Error>(volunteers);
        }
        catch (Exception ex)
        {
            _logger.Error("Error from Get Volunteers {error}", ex.Message);
            return new Error("error.get.volunteers", ex.Message, ErrorType.InternalServerError);
        }
    }
}