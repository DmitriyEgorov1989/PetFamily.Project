using PetFamily.Core.Application.UseCases.CommonDto;
using PetFamily.Core.Application.UseCases.Queries.GetAllPetsWithPaginationAndFilter;

namespace PetFamily.Api.Controllers.Models.Pets.ReadModels;

public class GetAllPetsWithPaginationAndFilterRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public Guid? VolunteerId { get; init; }
    public Guid? SpeciesId { get; init; }
    public Guid? BreedId { get; init; }
    public string City { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string House { get; init; } = string.Empty;
    public string SortBy { get; init; } = string.Empty;
    public string SortDirection { get; init; } = string.Empty;

    public GetAllPetsWithPaginationAndFilterQuery ToQuery()
    {
        return new GetAllPetsWithPaginationAndFilterQuery(
            new PaginationData(Page, PageSize),
            new FilterByData(
                Name,
                SpeciesId,
                BreedId,
                Color,
                VolunteerId,
                new AddressDto(City,
                    Region,
                    House)),
            new SortByData(
                SortBy,
                SortDirection));
    }
}