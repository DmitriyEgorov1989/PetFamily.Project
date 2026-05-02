using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Ports;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.ChangeStatusPet;

public class ChangeStatusPetHandler :
    IRequestHandler<ChangeStatusPetCommand, UnitResult<ErrorList>>
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ChangeStatusPetCommand> _validator;
    private readonly IVolunteerRepository _volunteerRepository;

    public ChangeStatusPetHandler(
        ILogger logger, IUnitOfWork unitOfWork,
        IValidator<ChangeStatusPetCommand> validator,
        IVolunteerRepository volunteerRepository)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _volunteerRepository = volunteerRepository;
    }

    public async Task<UnitResult<ErrorList>> Handle(ChangeStatusPetCommand command, CancellationToken cancellationToken)
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

        var resultChangeStatusPet = volunteer.ChangeStatusPet(
            PetId.Create(command.PetId).Value, Pet.ToHelpStatus(command.HelpStatus));

        if (resultChangeStatusPet.IsFailure)
        {
            _logger.Error("Error changing status of pet with id {id}",
                command.PetId);
            return (ErrorList)resultChangeStatusPet.Error;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<ErrorList>();
    }
}