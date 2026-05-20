using CSharpFunctionalExtensions;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.SharedKernel.Errors;
using System.Security.Claims;

namespace PetFamily.Accounts.Core.Ports;

public interface ITokenProvider
{
    string GenerateAccessToken(User user);
    Task<Result<string, Error>> GenerateRefreshToken(
        string token, User user, string? existingRefreshToken, CancellationToken cancellationToken);

    Task<Result<ClaimsPrincipal?, Error>> GetPrincipalFromToken(
        string token, CancellationToken cancellationToken = default);
}