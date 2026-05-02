using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.Entity.Pet;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate.VO.Pets;
using PetFamily.Volunteers.Core.Ports;
using Serilog;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.AddPet
{
    public class AddPetHandler : IRequestHandler<AddPetCommand, Result<Guid, Error.ErrorList>>
    {
        private readonly ILogger _logger;
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IValidator<AddPetCommand> _validator;

        public AddPetHandler(ILogger logger, IVolunteerRepository volunteerRepository, IValidator<AddPetCommand> validator)
        {
            _logger = logger;
            _volunteerRepository = volunteerRepository;
            _validator = validator;
        }

        public async Task<Result<Guid, Error.ErrorList>> Handle(
            AddPetCommand command, CancellationToken cancellationToken)
        {
            var resultValidation = await _validator.ValidateAsync(command, cancellationToken);

            if (!resultValidation.IsValid)
            {
                return resultValidation.ToValidationErrorResponse(command);
            }

            var volunteerId = VolunteerId.Create(command.VolunteerId).Value;

            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
            if (volunteer == null)
            {
                _logger.Information("Volunteer with id {id} not found", volunteerId);
                return (Error.ErrorList)GeneralErrors.NotFound(nameof(volunteerId));
            }

            var resultCreateNewPet = ToMap(volunteerId, command.PetWrite);
            if (resultCreateNewPet.IsFailure)
                return (Error.ErrorList)resultCreateNewPet.Error;
            var newPet = resultCreateNewPet.Value;

            volunteer.AddPet(newPet);
            await _volunteerRepository.SaveAsync(cancellationToken);
            _logger.Information(
                "Volunteer with id {volunteerId} success added new pet with id{petId}", volunteerId, newPet.Id);

            return (Guid)newPet.Id;
        }

        private Result<Pet, Error> ToMap(VolunteerId id, PetWriteDto writeDto)
        {
            var petId = PetId.NewId();

            var color = Color.Create(writeDto.Color).Value;

            var petSpecieInfo = PetSpeciesInfo.Create(
                   SpeciesId.Create(writeDto.SpeciesInfo.SpecieId).Value,
                   BreedId.Create(writeDto.SpeciesInfo.BreedId).Value).Value;

            var healthInfo = HealthInfo.Create(writeDto.HealthInfo).Value;

            var address =
                Address.Create(writeDto.Address.City, writeDto.Address.Region, writeDto.Address.House).Value;

            var phoneNumber = PhoneNumber.Create(writeDto.PhoneNumber).Value;

            var resultCreatePet = Pet.Create(
                petId,
                writeDto.Name,
                writeDto.Description,
                petSpecieInfo,
                color,
                healthInfo,
                address,
                writeDto.Weight,
                writeDto.Height,
                phoneNumber,
                writeDto.IsSterilized,
                writeDto.BirthDate,
                writeDto.IsVaccined,
                Pet.ToHelpStatus(writeDto.PetHelpStatus),
                writeDto.HelpRequisites.Select(
                    hr => HelpRequisite.Create(hr.Name, hr.Description).Value),
                id);

            if (resultCreatePet.IsFailure)
                return resultCreatePet.Error;

            _logger.Information("PetWrite create success with id {petId}", petId);
            return resultCreatePet;
        }
    }
}
