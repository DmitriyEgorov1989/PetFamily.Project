using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Core.Application.Extensions;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using PetFamily.Core.Ports;
using Primitives;
using Serilog;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.UpdatePet;

public class UpdatePetHandler : IRequestHandler<UpdatePetCommand, UnitResult<ErrorList>>
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdatePetCommand> _validator;
    private readonly IVolunteerRepository _volunteerRepository;

    public UpdatePetHandler(
        IVolunteerRepository volunteerRepository,
        ILogger logger,
        IValidator<UpdatePetCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _volunteerRepository = volunteerRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    ///     Хендлер для обновления информации о питомце, принадлежащем волонтеру. Он выполняет следующие шаги:
    ///     1. Валидирует команду обновления питомца.
    ///     2. Получает волонтера по идентификатору.
    ///     3. Обновляет информацию о питомце.
    ///     4. Сохраняет изменения в базе данных.
    /// </summary>
    /// <param name="command">Команда обновления информации о питомце.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат выполнения команды.</returns>
    public async Task<UnitResult<ErrorList>> Handle(
        UpdatePetCommand command, CancellationToken cancellationToken)
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

        var petId = PetId.Create(command.PetId).Value;
        var resultUpdadePetInfoById =
            volunteer.UpdatePetInfo(
                petId,
                command.Name,
                command.Description,
                command.Color,
                command.Address.City,
                command.Address.Region,
                command.Address.House,
                command.Weight,
                command.Height,
                command.IsSterilized,
                command.BirthDate,
                command.IsVaccined);

        if (resultUpdadePetInfoById.IsFailure)
        {
            _logger.Error("Failed to update pet info for pet with id {id}", command.PetId);
            return UnitResult.Failure(
                (ErrorList)resultUpdadePetInfoById.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<ErrorList>();
    }
}