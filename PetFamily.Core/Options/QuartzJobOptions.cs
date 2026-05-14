namespace PetFamily.Core.Options;

public class QuartzJobOptions
{
    public const string SECTION_NAME = "QuartzJob";
    public required string HardDeleteVolunteerIdentity { get; set; }
    public required string HardDeleteVolunteerTrigger { get; set; }
    public required string CronShedule { get; set; }
    public int StorageTimeDays { get; set; }
}