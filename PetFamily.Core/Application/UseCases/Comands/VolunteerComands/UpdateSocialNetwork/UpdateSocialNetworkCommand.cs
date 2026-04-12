using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.ComonDto;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Comands.Volunteer.UpdateSocialNetwork
{
    public record UpdateSocialNetworkCommand(Guid VolunteerId, List<SocialNetworkDto> SocialNetworks)
         : IRequest<UnitResult<ErrorList>>;
}
