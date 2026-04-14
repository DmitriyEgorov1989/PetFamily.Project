using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Core.Application.Extensions;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Ports;
using Primitives;
using Serilog;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.DeleteVolunteer
{
    public class DeleteVolunteerHandler : IRequestHandler<DeleteVolunteerCommand, UnitResult<ErrorList>>
    {
        private readonly ILogger _logger;
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IValidator<DeleteVolunteerCommand> _validator;

        public DeleteVolunteerHandler(
            ILogger logger,
            IVolunteerRepository volunteerRepository,
            IValidator<DeleteVolunteerCommand> validator)
        {
            _logger = logger;
            _volunteerRepository = volunteerRepository;
            _validator = validator;
        }

        public async Task<UnitResult<ErrorList>> Handle(
            DeleteVolunteerCommand command, CancellationToken cancellationToken)
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

            if (volunteer is null)
            {
                _logger.Information("Volunteer with {volunteerId} NOT FOUND", volunteerId);
                return UnitResult.Failure(
                    (ErrorList)GeneralErrors.NotFound(nameof(volunteerId)));
            }

            volunteer.Delete();
            cancellationToken.ThrowIfCancellationRequested();
            await _volunteerRepository.SaveAsync(cancellationToken);
            _logger.Information("Volunteer with {volunteerId} soft delete sucess", volunteerId);

            return UnitResult.Success<ErrorList>();
        }
    }
}