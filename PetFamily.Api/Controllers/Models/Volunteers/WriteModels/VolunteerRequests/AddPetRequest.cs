using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.AddPet;

namespace PetFamily.Api.Controllers.Models.Volunteers.WriteModels.VolunteerRequests
{
    public record AddPetRequest(PetDto Pet)
    {
        public AddPetCommand ToCommand(Guid VolunteerId) => new(VolunteerId, Pet);
    }
}