namespace PetFamily.Infrastructure.Options;

public class DataBaseOptions
{
    public const string SECTION_NAME = "Database";
    public string ConnectionString { get; init; }
}