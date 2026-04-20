using PetFamily.Core.Application.UseCases.CommonDto;

namespace PetFamily.Core.Application.UseCases.ComonDto;

public class VolunteerDto
{
    public VolunteerDto()
    {
    }

    public VolunteerDto(Guid
        volunteerId,
        string firstName,
        string middleName,
        string lastName,
        string email,
        string description,
        string experience,
        string phoneNumber,
        HelpRequisiteDto[] helpRequisites,
        SocialNetworkDto[] socialNetworks)
    {
        VolunteerId = volunteerId;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Email = email;
        Description = description;
        Experience = experience;
        PhoneNumber = phoneNumber;
        HelpRequisites = helpRequisites;
        SocialNetworks = socialNetworks;
    }

    public Guid VolunteerId { get; init; }
    public string FirstName { get; init; }
    public string MiddleName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Description { get; init; }
    public string Experience { get; init; }
    public string PhoneNumber { get; init; }
    public HelpRequisiteDto[] HelpRequisites { get; init; }
    public SocialNetworkDto[] SocialNetworks { get; init; }
}