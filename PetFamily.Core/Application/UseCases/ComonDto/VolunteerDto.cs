using PetFamily.Core.Application.UseCases.CommonDto;

namespace PetFamily.Core.Application.UseCases.ComonDto;

public class VolunteerDto
{
    public Guid VolunteerId { get; init; }
    public string FirstName { get; init; }
    public string MiddleName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Experience { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public HelpRequisiteDto[] HelpRequisites { get; init; } = [];
    public SocialNetworkDto[] SocialNetworks { get; init; } = [];
}