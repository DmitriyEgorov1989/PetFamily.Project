using PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.AddPet;

namespace PetFamily.Volunteers.Contracts.Controllers.Models.Volunteers.WriteModels.VolunteerRequests;

public record AddPetRequest(PetWriteDto PetWrite)
{
    public AddPetCommand ToCommand(Guid VolunteerId)
    {
        return new AddPetCommand(VolunteerId, PetWrite);
    }
}