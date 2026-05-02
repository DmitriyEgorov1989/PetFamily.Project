using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Ports;
using Serilog;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.DeletePet;

/// <summary>
///     Хендлер для обработки команды удаления питомца у волонтера.
///     Он выполняет валидацию команды, проверяет существование волонтера и удаляет питомца из его профиля.
///     Если операция успешна, сохраняет изменения в базе данных.
///     В случае ошибок логирует их и возвращает соответствующий результат.
/// </summary>
public class DeletePetHandler : IRequestHandler<DeletePetCommand, UnitResult<ErrorList>>
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeletePetCommand> _validator;
    private readonly IVolunteerRepository _volunteerRepository;

    public DeletePetHandler(
        ILogger logger,
        IVolunteerRepository volunteerRepository,
        IValidator<DeletePetCommand> validator, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _volunteerRepository = volunteerRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        DeletePetCommand command, CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(command, cancellationToken);

        if (!resultValidation.IsValid) return resultValidation.ToValidationErrorResponse(command);

        var volunteerId = VolunteerId.Create(command.VolunteerId).Value;

        var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId);
        if (volunteer is null)
        {
            _logger.Information("Volunteer with id {id} not found", volunteerId);
            return (ErrorList)GeneralErrors.NotFound(nameof(volunteerId));
        }

        var resultPetDelete =
            volunteer.DeletePet(PetId.Create(command.PetId).Value);
        if (resultPetDelete.IsFailure)
        {
            _logger.Error(
                "Error deleting pet with id {id} for volunteer with id {volunteerId}. Errors: {errors}",
                command.PetId, volunteerId, resultPetDelete.Error);
            return (ErrorList)resultPetDelete.Error;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<ErrorList>();
    }
}