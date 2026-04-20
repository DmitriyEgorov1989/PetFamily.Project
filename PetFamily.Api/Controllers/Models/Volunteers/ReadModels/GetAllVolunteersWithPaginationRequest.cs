using PetFamily.Core.Application.UseCases.Queries.GetAllVolunteersWithPagination;

namespace PetFamily.Api.Controllers.Models.Volunteers.ReadModels;

public record GetAllVolunteersWithPaginationRequest(int PageNumber, int PageSize)
{
    public GetAllVolunteersWithPaginationQuery ToQuery()
    {
        return new GetAllVolunteersWithPaginationQuery(PageNumber, PageSize);
    }
}