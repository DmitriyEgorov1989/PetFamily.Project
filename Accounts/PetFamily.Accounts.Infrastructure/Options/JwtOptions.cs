namespace PetFamily.Infrastructure.Options;

public class JwtOptions
{
    public const string SECTION_NAME = "JWT";
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string SecretKey { get; init; }
    public required int ExpirationMinutes { get; set; }
}