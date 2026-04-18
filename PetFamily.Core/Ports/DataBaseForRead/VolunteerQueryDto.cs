namespace PetFamily.Core.Ports.DataBaseForRead
{
    public class VolunteerQueryDto
    {
        public Guid VolunteerId { get; init; }
        public string FirstNameName { get; init; }
        public string MiddleName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Experience { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public HelpRequisiteQueryDto[] HelpRequisites { get; init; }
        public SocialNetworkQueryDto[] SocialNetworks { get; init; }
    }

    public record HelpRequisitesQueryDto(HelpRequisiteQueryDto[] ListHelpRequisites);

    public class HelpRequisiteQueryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class SocialNetworkQueryDto
    {
        public string Link { get; set; }
        public string Name { get; set; }
    }

    public class SocialNetworksQueryDto
    {
        public SocialNetworkQueryDto[] ListSocialNetworks { get; set; }
    }
}