using PetFamily.Core.Application.UseCases.CommonDto;

namespace PetFamily.Core.Application.UseCases.Queries.GetAllPetsWithPaginationAndFilter;

public record GetAllPetsWithPaginationAndFilterResponse(List<PetDto> PetList);