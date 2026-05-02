using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;

namespace PetFamily.Volunteers.Core.Ports.DataBaseForRead;

public interface IPetsReadRepository
{
    Task<Result<List<PetDto>, Error>> GetAllWithPaginationAndFiltersAsync(
        PaginationData paginationData,
        FilterByData? filterByData,
        SortByData? sortByData,
        CancellationToken cancellationToken = default);

    Task<Maybe<PetDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}