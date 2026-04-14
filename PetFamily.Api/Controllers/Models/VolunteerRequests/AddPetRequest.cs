using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.AddPet;

namespace PetFamily.Api.Controllers.Models.VolunteerRequests
{
    public record AddPetRequest(Guid VolunteerId, PetDto Pet);
}
