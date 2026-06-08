using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Accounts.Core.Domain.Models.Token;
using PetFamily.Accounts.Core.Ports;
using PetFamily.SharedKernel.Errors;

namespace PetFamily.Accounts.Infrastructure.Adapters.Postgres.Repositories.Write
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AccountDbContext _context;

        public RefreshTokenRepository(AccountDbContext context)
        {
            _context = context;
        }

        public async Task CreateTokenAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(token, cancellationToken);
        }

        public void Delete(RefreshToken token)
        {
            _context.Remove(token);
        }

        public async Task<Result<RefreshToken?, Error>> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var refreshToken =
                await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken == null)
            {
                return GeneralErrors.NotFound(token);
            }
            return refreshToken;
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}