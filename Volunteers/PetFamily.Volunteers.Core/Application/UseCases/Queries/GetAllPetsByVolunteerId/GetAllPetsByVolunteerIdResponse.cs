using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;

namespace PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllPetsByVolunteerId
{
    public record GetAllPetsByVolunteerIdResponse(List<PetDto> Pets);
}