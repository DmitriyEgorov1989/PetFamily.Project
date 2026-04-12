using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.ComonDto;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.CreateVolunteer
{
    public record CreateVolunteerCommand(
        FullNameDto FullName,
        string Email,
        string Description,
        int Experience,
        string PhoneNumber,
       List<SocialNetworkDto> SocialNetworks,
       List<HelpRequisiteDto> HelpRequisites
    ) : IRequest<Result<Guid, ErrorList>>;
}