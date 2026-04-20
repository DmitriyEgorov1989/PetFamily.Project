using PetFamily.Core.Application.UseCases.ComonDto;

namespace PetFamily.Core.Application.UseCases.Queries.GetAllVolunteersWithPagination;

public record GetAllVolunteersWithPaginationResponse(List<VolunteerDto> Volunteers);