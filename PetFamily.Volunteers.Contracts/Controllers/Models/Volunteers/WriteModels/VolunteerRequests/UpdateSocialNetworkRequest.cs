using PetFamily.Core.Application.UseCases.CommonDto;

namespace PetFamily.Volunteers.Contracts.Controllers.Models.Volunteers.WriteModels.VolunteerRequests;

public record UpdateSocialNetworkRequest(Guid VolunteerId, List<SocialNetworkDto> SocialNetworks);