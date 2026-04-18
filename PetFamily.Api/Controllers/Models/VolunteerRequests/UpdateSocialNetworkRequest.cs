using PetFamily.Core.Application.UseCases.CommonDto;

namespace PetFamily.Api.Controllers.Models.VolunteerRequests
{
    public record UpdateSocialNetworkRequest(Guid VolunteerId, List<SocialNetworkDto> SocialNetworks);
}