using PetFamily.Core.Application.UseCases.CommonDto;

namespace PetFamily.Core.Application.UseCases.ComonDto;

public record VolunteerDto(
    Guid VolunteerId,
    string FirstName,
    string MiddleName,
    string LastName,
    string Email,
    string Description,
    string Experience,
    string PhoneNumber,
    HelpRequisiteDto[] HelpRequisites,
    SocialNetworkDto[] SocialNetworks);