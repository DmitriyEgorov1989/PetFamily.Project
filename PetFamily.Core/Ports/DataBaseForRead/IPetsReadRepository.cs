using CSharpFunctionalExtensions;
using PetFamily.Core.Application.UseCases.CommonDto;
using Primitives;

namespace PetFamily.Core.Ports.DataBaseForRead
{
    public interface IPetsReadRepository
    {
        Task<Result<List<PetDto>, Error>> GetAllWithPaginationAndFiltersAsync(
            PaginationData paginationData,
            FilterByData? filterByData,
            SortByData? sortByData,
            CancellationToken cancellationToken = default);

        Task<Maybe<PetDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
