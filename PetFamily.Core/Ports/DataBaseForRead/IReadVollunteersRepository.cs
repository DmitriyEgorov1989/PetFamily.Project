using CSharpFunctionalExtensions;
using PetFamily.Core.Application.UseCases.CommonDto;
using Primitives;

namespace PetFamily.Core.Ports.DataBaseForRead;

public interface IReadVollunteersRepository
{
    Task<Result<IEnumerable<VolunteerDto>, Error>> GetAllVolunteersWithPaginationAsync(
        int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<Maybe<VolunteerDto>, Error>> GetVolunteerById(
        Guid volunteerId, CancellationToken cancellationToken = default);
}