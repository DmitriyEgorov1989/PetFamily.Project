using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;

namespace PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllVolunteersWithPagination;

public record GetAllVolunteersWithPaginationResponse(List<VolunteerDto> Volunteers);