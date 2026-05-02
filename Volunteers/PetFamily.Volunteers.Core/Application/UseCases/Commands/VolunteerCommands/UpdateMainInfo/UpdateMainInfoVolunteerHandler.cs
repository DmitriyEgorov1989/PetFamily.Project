using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Ports;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.Comands.Volunteer.UpdateMainInfo;

public class UpdateMainInfoVolunteerHandler :
    IRequestHandler<UpdateMainInfoVolunteerCommand, UnitResult<ErrorList>>
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateMainInfoVolunteerCommand> _validator;
    private readonly IVolunteerRepository _volunteerRepository;

    public UpdateMainInfoVolunteerHandler(
        IVolunteerRepository volunteerRepository,
        ILogger logger,
        IValidator<UpdateMainInfoVolunteerCommand> validator,
        IUnitOfWork unitOfWork)

    {
        _volunteerRepository = volunteerRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        UpdateMainInfoVolunteerCommand command,
        CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(command, cancellationToken);

        if (!resultValidation.IsValid)
            return UnitResult.Failure(
                resultValidation.ToValidationErrorResponse(command));

        var volunteerId = VolunteerId.Create(command.VolunteerId).Value;

        cancellationToken.ThrowIfCancellationRequested();
        var volunteer =
            await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);

        if (volunteer == null)
        {
            _logger.Error("Volunteer with id {id} not found", volunteerId);
            return UnitResult.Failure(
                (ErrorList)GeneralErrors.NotFound(nameof(volunteerId)));
        }

        var resultUpdateMainInfo =
            volunteer.UpdateMainIfo(
                command.UpdateMainInfo.FullName?.FirstName,
                command.UpdateMainInfo.FullName?.MiddleName,
                command.UpdateMainInfo.FullName?.LastName,
                command.UpdateMainInfo.Email,
                command.UpdateMainInfo.Description,
                command.UpdateMainInfo.Experience,
                command.UpdateMainInfo.PhoneNumber);

        if (resultUpdateMainInfo.IsFailure)
        {
            _logger.Error("An error occurred while updating volunteer information. ");
            return
                UnitResult.Failure((ErrorList)resultUpdateMainInfo.Error);
        }

        cancellationToken.ThrowIfCancellationRequested();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.Information("Main info user with id {id} updates success", volunteerId);

        return UnitResult.Success<ErrorList>();
    }
}