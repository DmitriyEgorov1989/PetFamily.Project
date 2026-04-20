using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Core.Application.Extensions;
using PetFamily.Core.Ports.DataBaseForRead;
using Primitives;
using Serilog;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Queries.GetVolunteerById;

public class GetVolunteerByIdHandler :
    IRequestHandler<GetVolunteerByIdQuery, Result<GetVolunteerByIdResponse, ErrorList>>
{
    private readonly ILogger _logger;
    private readonly IReadRepository _readRepository;
    private readonly IValidator<GetVolunteerByIdQuery> _validator;

    public GetVolunteerByIdHandler(
        IReadRepository readRepository,
        ILogger logger,
        IValidator<GetVolunteerByIdQuery> validator)
    {
        _readRepository = readRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<GetVolunteerByIdResponse, ErrorList>> Handle(GetVolunteerByIdQuery query,
        CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(query, cancellationToken);

        if (!resultValidation.IsValid) return resultValidation.ToValidationErrorResponse(query);

        cancellationToken.ThrowIfCancellationRequested();
        var resultGetVolunteer =
            await _readRepository.GetVolunteerById(query.VolunteerId, cancellationToken);
        if (resultGetVolunteer.IsFailure)
        {
            _logger.Error("Error get volunteer by id {error}", resultGetVolunteer.Error);
            return (ErrorList)resultGetVolunteer.Error;
        }

        if (resultGetVolunteer.Value.HasNoValue)
        {
            _logger.Information("Volunteer with {id} not found", query.VolunteerId);
            return (ErrorList)GeneralErrors.NotFound(nameof(query.VolunteerId));
        }

        return new GetVolunteerByIdResponse(resultGetVolunteer.Value.Value);
    }
}