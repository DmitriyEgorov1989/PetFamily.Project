using PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllVolunteersWithPagination;

namespace PetFamily.Volunteers.Contracts.Controllers.Models.Volunteers.ReadModels;

public record GetAllVolunteersWithPaginationRequest(int PageNumber, int PageSize)
{
    public GetAllVolunteersWithPaginationQuery ToQuery()
    {
        return new GetAllVolunteersWithPaginationQuery(PageNumber, PageSize);
    }
}