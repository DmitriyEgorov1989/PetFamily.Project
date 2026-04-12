using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.ComonDto;

namespace PetFamily.Api.Controllers.Models.VolunteerRequests
{
    public record UpdateSocialNetworkRequest(Guid VolunteerId, List<SocialNetworkDto> SocialNetworks);
}