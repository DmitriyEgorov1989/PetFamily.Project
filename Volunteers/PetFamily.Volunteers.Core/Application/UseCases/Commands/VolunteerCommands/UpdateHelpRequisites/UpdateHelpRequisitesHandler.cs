using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Ports;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.UpdateHelpRequisites;

public class UpdateHelpRequisitesHandler :
    IRequestHandler<UpdateHelpRequisitesCommand, UnitResult<ErrorList>>
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateHelpRequisitesCommand> _validator;
    private readonly IVolunteerRepository _volunteerRepository;

    public UpdateHelpRequisitesHandler(
        IVolunteerRepository volunteerRepository,
        ILogger logger,
        IValidator<UpdateHelpRequisitesCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _volunteerRepository = volunteerRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        UpdateHelpRequisitesCommand command, CancellationToken cancellationToken)
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

        var helpRequisites =
            HelpRequisites.Create(
                command.HelpRequisites.Select(hr => HelpRequisite.Create(hr.Name, hr.Description).Value));

        volunteer.UpdateHelpRequisites(helpRequisites);

        cancellationToken.ThrowIfCancellationRequested();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.Information("Main info user with id {id} updates success", volunteerId);

        return UnitResult.Success<ErrorList>();
    }
}