using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Ports;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.CreateVolunteer;

/// <summary>
///     Handler для создания волантера
/// </summary>
public class CreateVolunteerHandler : IRequestHandler<CreateVolunteerCommand, Result<Guid, ErrorList>>
{
    private readonly ILogger _logger;
    private readonly IValidator<CreateVolunteerCommand> _validator;
    private readonly IVolunteerRepository _volunteerRepository;

    /// <summary>
    ///     ctor
    /// </summary>
    /// <param name="volunteerRepository">Порт для репозитория волонтера</param>
    public CreateVolunteerHandler(
        IVolunteerRepository volunteerRepository
        , ILogger logger,
        IValidator<CreateVolunteerCommand> validator)
    {
        _volunteerRepository = volunteerRepository;
        _logger = logger;
        _validator = validator;
    }

    /// <summary>
    ///     Use cases mediatr для создания волонтера
    /// </summary>
    /// <param name="request">данные для создания приходят с фронта</param>
    /// <param name="cancellationToken">токен отмены операции</param>
    /// <returns>возвращает Guid или Error</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateVolunteerCommand command, CancellationToken cancellationToken = default)
    {
        var resultValidation = await _validator.ValidateAsync(command, cancellationToken);

        if (!resultValidation.IsValid) return resultValidation.ToValidationErrorResponse(command);
        var createVolunteer = MapTo(command);
        if (createVolunteer.IsFailure)
            return (ErrorList)createVolunteer.Error;

        var newVolunteer = createVolunteer.Value;

        cancellationToken.ThrowIfCancellationRequested();

        await _volunteerRepository.AddAsync(newVolunteer, cancellationToken);
        _logger.Information("Volonteer create with {id} ", (Guid)newVolunteer.Id);
        return (Guid)newVolunteer.Id;
    }

    /// <summary>
    ///     Метод для мапинга request в данные для создания волонтера
    /// </summary>
    /// <param name="request">прихидящие данные</param>
    /// <returns>Модель волонтера либо Error</returns>
    private Result<Volunteer, Error> MapTo(CreateVolunteerCommand request)
    {
        var fullName = FullName.Create(
            request.FullName.FirstName,
            request.FullName.MiddleName,
            request.FullName.LastName).Value;
        var email = Email.Create(request.Email).Value;
        var experience = Experience.Create(request.Experience).Value;
        var phoneNumber = PhoneNumber.Create(request.PhoneNumber).Value;
        var helpRequisites = HelpRequisites.Create(request.HelpRequisites.Select(hr =>
        {
            return HelpRequisite.Create(hr.Name, hr.Description).Value;
        }));
        var socialNetworks = SocialNetworks.Create(request.SocialNetworks.Select(s =>
        {
            return SocialNetwork.Create(s.Name, s.Link).Value;
        }));
        var volunteerId = VolunteerId.NewId();
        var resultCreateVolunteer = Volunteer.Create(
            volunteerId,
            fullName,
            email,
            request.Description,
            experience,
            phoneNumber,
            helpRequisites,
            socialNetworks);

        if (resultCreateVolunteer.IsFailure)
            return resultCreateVolunteer.Error;

        return resultCreateVolunteer.Value;
    }
}