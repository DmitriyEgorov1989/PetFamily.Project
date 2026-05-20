using Microsoft.IdentityModel.Tokens;
using PetFamily.Core.Options;
using System.Text;

namespace PetFamily.SharedKernel.Extensions.Validations
{
    public static class TokenValidationsParametersExtension
    {
        public static TokenValidationParameters GetValidationsParametersWithLifeTime(
            this TokenValidationParameters tokenValidationsParameters, JwtOptions jwtOptions)
        {
            tokenValidationsParameters.ValidateIssuer = true;
            tokenValidationsParameters.ValidateAudience = true;
            tokenValidationsParameters.ValidateLifetime = true;
            tokenValidationsParameters.ValidateIssuerSigningKey = true;
            tokenValidationsParameters.ValidIssuer = jwtOptions.Issuer;
            tokenValidationsParameters.ValidAudience = jwtOptions.Audience;
            tokenValidationsParameters.IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
            tokenValidationsParameters.ClockSkew = TimeSpan.Zero;
            return tokenValidationsParameters;
        }

        public static TokenValidationParameters GetValidationsParameters(
            this TokenValidationParameters tokenValidationsParameters, JwtOptions jwtOptions)
        {
            tokenValidationsParameters.ValidateIssuer = true;
            tokenValidationsParameters.ValidateAudience = true;
            tokenValidationsParameters.ValidateIssuerSigningKey = true;
            tokenValidationsParameters.ValidIssuer = jwtOptions.Issuer;
            tokenValidationsParameters.ValidAudience = jwtOptions.Audience;
            tokenValidationsParameters.IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
            tokenValidationsParameters.ClockSkew = TimeSpan.Zero;
            return tokenValidationsParameters;
        }
    }
}