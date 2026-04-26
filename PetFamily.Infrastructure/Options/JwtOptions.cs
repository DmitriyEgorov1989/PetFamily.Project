namespace PetFamily.Infrastructure.Options
{
    public class JwtOptions
    {
        public const string SECTION_NAME = "JWT";
        public string Issuer { get; init; }
        public string Audience { get; init; }
        public string SecretKey { get; init; }
        public int ExpirationMinutes { get; set; }
    }
}