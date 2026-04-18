using CSharpFunctionalExtensions;
using PetFamily.Core.Application.UseCases.ComonDto;
using Primitives;

namespace PetFamily.Core.Ports.DataBaseForRead;

public interface IReadRepository
{
    Task<Result<IEnumerable<VolunteerDto>, Error>> GetAllVolunteersAsync(
        CancellationToken cancellationToken = default);
}