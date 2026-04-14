using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Core.Application.Extensions;
using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.AddPet;
using PetFamily.Core.Domain.Models.PetAggregate;
using PetFamily.Core.Domain.Models.SharedKernel.VO;
using PetFamily.Core.Domain.Models.SpeciesAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;
using PetFamily.Core.Ports;
using Primitives;
using Serilog;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.AddPet
{
    public class AddPetHandler : IRequestHandler<AddPetCommand, Result<Guid, ErrorList>>
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

        public async Task<Result<Guid, ErrorList>> Handle(
            AddPetCommand command, CancellationToken cancellationToken)
        {
            var resultValidation = await _validator.ValidateAsync(command, cancellationToken);

            if (!resultValidation.IsValid)
            {
                return resultValidation.ToValidationErrorResponse(command);
            }

            var volunteerId = VolunteerId.Create(command.VolunteerId).Value;

            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId);
            if (volunteer is null)
            {
                _logger.Information("Volunteer with id {id} not found", volunteerId);
                return (ErrorList)GeneralErrors.NotFound(nameof(volunteerId));
            }

            var resultCreateNewPet = ToMap(volunteerId, command.Pet);
            if (resultCreateNewPet.IsFailure)
                return (ErrorList)resultCreateNewPet.Error;
            var newPet = resultCreateNewPet.Value;

            volunteer.AddPet(newPet);
            await _volunteerRepository.SaveAsync(cancellationToken);
            _logger.Information(
                "Volunteer with id {volunteerId} success aded new pet with id{petId}", volunteerId, newPet.Id);

            return (Guid)newPet.Id;
        }

        private Result<Pet, Error> ToMap(VolunteerId id, PetDto dto)
        {
            var petId = PetId.NewId();

            var color = Color.Create(dto.Color).Value;

            var petSpecieInfo = PetSpeciesInfo.Create(
                   SpeciesId.Create(dto.SpeciesInfo.SpecieId).Value,
                   BreedId.Create(dto.SpeciesInfo.BreedId).Value).Value;

            var healthInfo = HealthInfo.Create(dto.HealthInfo).Value;

            var address =
                Address.Create(dto.Address.City, dto.Address.Region, dto.Address.House).Value;

            var phoneNumber = PhoneNumber.Create(dto.PhoneNumber).Value;

            var resultCreatePet = Pet.Create(
                petId,
                dto.Name,
                dto.Description,
                petSpecieInfo,
                color,
                healthInfo,
                address,
                dto.Weight,
                dto.Height,
                phoneNumber,
                dto.IsSterilized,
                dto.BirthDate,
                dto.IsVaccined,
                Pet.ToHelpStatus(dto.PetHelpStatus),
                HelpRequisites.Create(null),
                PetPhotos.Create(null),
                id);

            if (resultCreatePet.IsFailure)
                return resultCreatePet.Error;

            _logger.Information("Pet create sucess with id {petId}", petId);
            return resultCreatePet;
        }
    }
}