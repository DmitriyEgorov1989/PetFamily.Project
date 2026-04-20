using PetFamily.Core.Application.UseCases.CommonDto;

namespace PetFamily.Api.Controllers.Models.Volunteers.WriteModels.VolunteerRequests
{
    public record UpdateSocialNetworkRequest(Guid VolunteerId, List<SocialNetworkDto> SocialNetworks);
}