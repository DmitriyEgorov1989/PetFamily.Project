namespace PetFamily.Core.Options;

public class MinioOptions
{
    public const string SECTION_NAME = "Minio";
    public required string ConnectionString { get; init; }
    public required string Login { get; init; }
    public required string Password { get; init; }
    public required string DefaultBucket { get; init; }
}