using Microsoft.EntityFrameworkCore;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Ports;

namespace PetFamily.Infrastructure.Adapters.Postgres.WriteDataBase.Repository
{
    public class VolonteerRepository : IVolunteerRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public VolonteerRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> AddAsync(Volunteer volonteer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _dbContext.AddAsync(volonteer, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return (Guid)volonteer.Id;
        }

        public async Task<List<Volunteer>> GetAllDeleteAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Volunteers
                                .Where(v => v.IsDelete)
                                .Include(v => v.Pets).ToListAsync();
        }

        public async Task<Volunteer> GetByIdAsync(VolunteerId volunteerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Volunteers.
                                     Include(v => v.Pets).ThenInclude(p => p.Photos)
                                     .FirstOrDefaultAsync(v => v.Id == volunteerId);
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(VolunteerId id, CancellationToken cancellationToken = default)
        {
            await _dbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == id);
            await _dbContext.SaveChangesAsync();
        }
    }
}