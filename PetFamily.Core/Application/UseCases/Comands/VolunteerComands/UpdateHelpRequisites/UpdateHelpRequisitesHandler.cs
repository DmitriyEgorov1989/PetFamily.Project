using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Core.Application.Extensions;
using PetFamily.Core.Domain.Models.SharedKernel.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Ports;
using Primitives;
using Serilog;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.UpdateHelpRequisites
{
    public class UpdateHelpRequisitesHandler :
        IRequestHandler<UpdateHelpRequisitesCommand, UnitResult<ErrorList>>
    {
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly ILogger _logger;
        private readonly IValidator<UpdateHelpRequisitesCommand> _validator;
        public UpdateHelpRequisitesHandler(IVolunteerRepository volunteerRepository, ILogger logger, IValidator<UpdateHelpRequisitesCommand> validator)
        {
            _volunteerRepository = volunteerRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<UnitResult<ErrorList>> Handle(
            UpdateHelpRequisitesCommand command, CancellationToken cancellationToken)
        {
            var resultValidation = await _validator.ValidateAsync(command, cancellationToken);

            if (!resultValidation.IsValid)
            {
                return UnitResult.Failure(
                    resultValidation.ToValidationErrorResponse(command));
            }

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
                    command.HelpRequisites.Select(
                       hr => HelpRequisite.Create(hr.Name, hr.Description).Value));

            volunteer.UpdateHelpRequisites(helpRequisites);

            cancellationToken.ThrowIfCancellationRequested();
            await _volunteerRepository.SaveAsync(cancellationToken);
            _logger.Information("Main info user with id {id} updates succes", volunteerId);

            return UnitResult.Success<ErrorList>();
        }
    }
}
