using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Accounts.Core.Domain.Models;
using PetFamily.Accounts.Core.Domain.Models.Token;
using PetFamily.Accounts.Core.Ports;
using PetFamily.Core.Options;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetFamily.Accounts.Infrastructure.Adapters.Jwt;

public class TokenProvider : ITokenProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly RefreshTokenOptions _refreshTokenOptions;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public TokenProvider(IOptions<JwtOptions> options,
        IOptions<RefreshTokenOptions> refreshTokenOptions,
        IRefreshTokenRepository refreshTokenRepository
)
    {
        _jwtOptions = options.Value;
        _refreshTokenOptions = refreshTokenOptions.Value;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenValidationParameters =
            new TokenValidationParameters().GetValidationsParameters(options.Value);
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new(CustomClaims.UserId, user.Id.ToString()),
            new(CustomClaims.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(CustomClaims.Role, user.RoleId.ToString() ?? "")
        };
        var jwt = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_jwtOptions.ExpirationMinutes)),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public async Task<Result<RefreshToken, Error>> GenerateRefreshToken(
        string token, User user, CancellationToken cancellationToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        if (jwtToken is null)
            return GeneralErrors.InternalServerError("Error read jwtToken");
        var jwtId = jwtToken.Id;

        var resultCreateRefreshToken =
            RefreshToken.Create(
            Guid.NewGuid().ToString(),
                jwtId,
       DateTime.UtcNow.AddDays(_refreshTokenOptions.ExpirationDay),
       user.Id);
        if (resultCreateRefreshToken.IsFailure)
            return resultCreateRefreshToken.Error;

        var refreshToken = resultCreateRefreshToken.Value;
        return refreshToken;
    }

    public async Task<Result<ClaimsPrincipal?, Error>> GetPrincipalFromToken(
        string token, CancellationToken cancellationToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var tokenParameters = _tokenValidationParameters.Clone();
            tokenParameters.ValidateLifetime = false;
            var principal =
                tokenHandler.ValidateToken(
                    token,
                    tokenParameters,
                    out var validatedToken);
            return IsJwtWithValidSecurityAlgorithm(validatedToken) ? principal : null;
        }
        catch (Exception ex)
        {
            return GeneralErrors.InternalServerError(ex.Message);
        }
    }

    private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    => validatedToken is JwtSecurityToken jwtSecurityToken
       && jwtSecurityToken.Header.Alg
            .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
}