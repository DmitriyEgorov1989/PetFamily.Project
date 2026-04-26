using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Core.Domain.Models.AccountAggregate;
using PetFamily.Core.Ports;
using PetFamily.Infrastructure.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetFamily.Infrastructure.Adapters.Jwt;

internal class JwtTokenProvider : ITokenProvider
{
    private readonly JwtOptions _options;

    public JwtTokenProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateAccessToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim("Id", user.Id.ToString()),
            new Claim("Email", user.Email ?? "")
        };
        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_options.ExpirationMinutes)),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256)
            );
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}