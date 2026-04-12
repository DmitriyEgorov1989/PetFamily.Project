namespace PetFamily.Infrastructure.Options
{
    public class MinioOptions
    {
        public const string SECTION_NAME = "Minio";
        public string ConnectionString { get; init; }
        public string Login { get; init; }
        public string Password { get; init; }
        public string DefaultBucket { get; init; }
    }
}