using CSharpFunctionalExtensions;
using PetFamily.Accounts.Core.Domain.Models.Token;
using PetFamily.SharedKernel.Errors;

namespace PetFamily.Accounts.Core.Ports
{
    public interface IRefreshTokenRepository
    {
        Task CreateTokenAsync(RefreshToken token, CancellationToken cancellationToken = default);
        Task<Result<RefreshToken?, Error>> GetByTokenAsync(
            string token, CancellationToken cancellationToken = default);
        void Delete(RefreshToken token);

        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}