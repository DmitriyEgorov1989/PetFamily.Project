using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Errors;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Accounts.Core.Domain.Models.Token;

public class RefreshToken
{
    [ExcludeFromCodeCoverage]
    private RefreshToken()
    {
    }

    public Guid Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public string JwtId { get; private set; } = string.Empty;
    public DateTime ExpiryDate { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public bool Invalidated { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public bool IsExpired => ExpiryDate <= DateTime.UtcNow;
    public bool IsActive => !Invalidated && !IsExpired;

    public static Result<RefreshToken, Error> Create(
        string token,
        string jwtId,
        DateTime expiryDate,
        Guid userId)
    {
        if (string.IsNullOrWhiteSpace(token))
            return GeneralErrors.ValueIsRequired(nameof(token));

        if (string.IsNullOrWhiteSpace(jwtId))
            return GeneralErrors.ValueIsRequired(nameof(jwtId));

        if (expiryDate <= DateTime.UtcNow)
            return GeneralErrors.ValueIsInvalid(nameof(expiryDate));

        if (userId == Guid.Empty)
            return GeneralErrors.ValueIsRequired(nameof(userId));

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            JwtId = jwtId,
            ExpiryDate = expiryDate,
            CreatedAtUtc = DateTime.UtcNow,
            Invalidated = false,
            UserId = userId
        };
    }

    public UnitResult<Error> Invalidate()
    {
        if (Invalidated)
            return GeneralErrors.Failure("Refresh token is already invalidated");

        Invalidated = true;
        UpdatedAtUtc = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Update(string token, string jwtId, DateTime expiryDate)
    {
        if (string.IsNullOrWhiteSpace(token))
            return GeneralErrors.ValueIsRequired(nameof(token));

        if (string.IsNullOrWhiteSpace(jwtId))
            return GeneralErrors.ValueIsRequired(nameof(jwtId));

        if (expiryDate <= DateTime.UtcNow)
            return GeneralErrors.ValueIsInvalid(nameof(expiryDate));

        Token = token;
        JwtId = jwtId;
        ExpiryDate = expiryDate;
        Invalidated = false;
        UpdatedAtUtc = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }
}
