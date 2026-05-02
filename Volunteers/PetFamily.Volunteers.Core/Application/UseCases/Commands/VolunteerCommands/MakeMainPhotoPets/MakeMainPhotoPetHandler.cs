using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Ports;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.MakeMainPhotoPets;

public class MakeMainPhotoPetHandler :
    IRequestHandler<MakeMainPhotoPetCommand, Result<string, ErrorList>>
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<MakeMainPhotoPetCommand> _validator;
    private readonly IVolunteerRepository _volunteerRepository;

    public MakeMainPhotoPetHandler(
        IVolunteerRepository volunteerRepository,
        ILogger logger,
        IValidator<MakeMainPhotoPetCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _volunteerRepository = volunteerRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string, ErrorList>> Handle(
        MakeMainPhotoPetCommand command, CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(command, cancellationToken);

        if (!resultValidation.IsValid) return resultValidation.ToValidationErrorResponse(command);

        var volunteerId = VolunteerId.Create(command.VolunteerId).Value;

        cancellationToken.ThrowIfCancellationRequested();
        var volunteer =
            await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);

        if (volunteer == null)
        {
            _logger.Error("Volunteer with id {id} not found", volunteerId);
            return (ErrorList)GeneralErrors.NotFound(nameof(volunteerId));
        }

        var resultMakeMainPhotoPet = volunteer.MakeMainPhotoPet(
            PetId.Create(command.PetId).Value, command.PathStorage);
        if (resultMakeMainPhotoPet.IsFailure)
        {
            _logger.Error("Failed to make main photo for pet with id {id}",
                command.PetId);
            return (ErrorList)resultMakeMainPhotoPet.Error;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return command.PathStorage;
    }
}