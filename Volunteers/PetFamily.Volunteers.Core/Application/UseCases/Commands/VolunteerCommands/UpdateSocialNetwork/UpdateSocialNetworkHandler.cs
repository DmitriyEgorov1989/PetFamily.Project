using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Ports;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.Comands.Volunteer.UpdateSocialNetwork;

public class UpdateSocialNetworkHandler : IRequestHandler<UpdateSocialNetworkCommand, UnitResult<ErrorList>>
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateSocialNetworkCommand> _validator;
    private readonly IVolunteerRepository _volunteerRepository;

    public UpdateSocialNetworkHandler(
        IVolunteerRepository volunteerRepository,
        ILogger logger,
        IValidator<UpdateSocialNetworkCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _volunteerRepository = volunteerRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        UpdateSocialNetworkCommand command, CancellationToken cancellationToken)
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
            return UnitResult.Failure((ErrorList)GeneralErrors.NotFound(nameof(volunteerId)));
        }

        var socialNetWorks =
            SocialNetworks.Create(
                command.SocialNetworks.Select(sn => SocialNetwork.Create(sn.Name, sn.Link).Value));

        volunteer.UpdateSocialNetworks(socialNetWorks);

        cancellationToken.ThrowIfCancellationRequested();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.Information("Main info user with id {id} updates success", volunteerId);

        return UnitResult.Success<ErrorList>();
    }
}