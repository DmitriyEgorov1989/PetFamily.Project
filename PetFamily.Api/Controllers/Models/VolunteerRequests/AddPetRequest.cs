using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.AddPet;

namespace PetFamily.Api.Controllers.Models.VolunteerRequests
{
    public record AddPetRequest(Guid VolunteerId, PetDto Pet);
}
