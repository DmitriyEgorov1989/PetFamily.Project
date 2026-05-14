namespace PetFamily.Core.Options;

public class DataBaseOptions
{
    public const string SECTION_NAME = "Database";
    public required string ConnectionString { get; init; }
}