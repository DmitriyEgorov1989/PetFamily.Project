namespace PetFamily.Core.Options
{
    public class RefreshTokenOptions
    {
        public const string SECTION_NAME = "RefreshToken";
        public int ExpirationDay { get; set; }
    }
}