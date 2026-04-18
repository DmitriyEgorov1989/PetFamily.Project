using PetFamily.Core.Application.UseCases.ComonDto;

namespace PetFamily.Core.Application.UseCases.Queries.GetAllVolunteers;

public record GetAllVolunteersResponse(List<VolunteerDto> Volunteers);