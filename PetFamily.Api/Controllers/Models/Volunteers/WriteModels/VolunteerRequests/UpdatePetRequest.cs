using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.UpdatePet;
using PetFamily.Core.Application.UseCases.CommonDto;

namespace PetFamily.Api.Controllers.Models.Volunteers.WriteModels.VolunteerRequests;

public record UpdatePetRequest(
    string Name,
    string Description,
    string Color,
    string HealthInfo,
    AddressDto Address,
    decimal Weight,
    int Height,
    string PhoneNumber,
    bool IsSterilized,
    DateTime BirthDate,
    bool IsVaccined)
{
    public UpdatePetCommand ToCommand(Guid petId, Guid volunteerId)
    {
        return new UpdatePetCommand(
            petId,
            volunteerId,
            Name,
            Description,
            Color,
            HealthInfo,
            new AddressDto(Address.City, Address.Region, Address.House),
            Weight,
            Height,
            PhoneNumber,
            IsSterilized,
            BirthDate,
            IsVaccined);
    }
}