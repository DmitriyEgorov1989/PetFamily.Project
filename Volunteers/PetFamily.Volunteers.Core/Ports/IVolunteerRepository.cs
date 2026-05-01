using PetFamily.Core.Domain.Models.VolunteerAggregate;
using PetFamily.SharedKernel.DomainModels.Ids;

namespace PetFamily.Core.Ports;

public interface IVolunteerRepository
{
    Task<Guid> AddAsync(Volunteer volunteer, CancellationToken cancellationToken = default);
    Task<Volunteer> GetByIdAsync(VolunteerId volunteer, CancellationToken cancellationToken = default);
    Task SaveAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(VolunteerId id, CancellationToken cancellationToken = default);
    Task<List<Volunteer>> GetAllDeleteAsync(CancellationToken cancellationToken = default);
}