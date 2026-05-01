namespace PetFamily.Infrastructure.Options;

public class QuartzJobOptions
{
    public const string SECTION_NAME = "QuartzJob";
    public string HardDeleteVolunteerIdentity { get; set; }
    public string HardDeleteVolunteerTrigger { get; set; }
    public string CronShedule { get; set; }
    public int StorageTimeDays { get; set; }
}