using PetFamily.Core.Application.UseCases.CommonDto;

namespace PetFamily.Core.Application.UseCases.Queries.GetAllVolunteersWithPagination;

public record GetAllVolunteersWithPaginationResponse(List<VolunteerDto> Volunteers);