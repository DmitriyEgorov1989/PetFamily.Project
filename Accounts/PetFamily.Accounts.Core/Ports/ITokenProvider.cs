using CSharpFunctionalExtensions;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.Accounts.Core.Domain.Models.Token;
using PetFamily.SharedKernel.Errors;
using System.Security.Claims;

namespace PetFamily.Accounts.Core.Ports;

public interface ITokenProvider
{
    string GenerateAccessToken(User user);
    Task<Result<RefreshToken, Error>> GenerateRefreshToken(
        string token, User usern, CancellationToken cancellationToken);

    Task<Result<ClaimsPrincipal?, Error>> GetPrincipalFromToken(
        string token, CancellationToken cancellationToken = default);
}