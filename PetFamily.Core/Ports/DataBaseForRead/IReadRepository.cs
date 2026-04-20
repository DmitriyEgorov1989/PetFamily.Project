using CSharpFunctionalExtensions;
using PetFamily.Core.Application.UseCases.ComonDto;
using Primitives;

namespace PetFamily.Core.Ports.DataBaseForRead;

public interface IReadRepository
{
    Task<Result<IEnumerable<VolunteerDto>, Error>> GetAllVolunteersWithPaginationAsync(
        int page, int pageSize, CancellationToken cancellationToken = default);
}