using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Core.Application.Extensions;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Ports;
using Primitives;
using Serilog;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Comands.Volunteer.UpdateSocialNetwork
{
    public class UpdateSocialNetworkHandler : IRequestHandler<UpdateSocialNetworkCommand, UnitResult<ErrorList>>
    {
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly ILogger _logger;
        private readonly IValidator<UpdateSocialNetworkCommand> _validator;

        public UpdateSocialNetworkHandler(IVolunteerRepository volunteerRepository, ILogger logger, IValidator<UpdateSocialNetworkCommand> validator)
        {
            _volunteerRepository = volunteerRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<UnitResult<ErrorList>> Handle(
            UpdateSocialNetworkCommand command, CancellationToken cancellationToken)
        {
            var resultValidation = await _validator.ValidateAsync(command, cancellationToken);

            if (!resultValidation.IsValid)
            {
                return resultValidation.ToValidationErrorResponse(command);
            }
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
                    command.SocialNetworks.Select(
                        sn => SocialNetwork.Create(sn.Name, sn.Link).Value));

            volunteer.UpdateSocialNetworks(socialNetWorks);

            cancellationToken.ThrowIfCancellationRequested();
            await _volunteerRepository.SaveAsync(cancellationToken);
            _logger.Information("Main info user with id {id} updates succes", volunteerId);

            return UnitResult.Success<ErrorList>();
        }
    }
}