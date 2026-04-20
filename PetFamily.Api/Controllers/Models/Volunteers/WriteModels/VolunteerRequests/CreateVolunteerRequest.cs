using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Application.UseCases.CommonDto;
using static Primitives.Error;

namespace PetFamily.Api.Controllers.Models.Volunteers.WriteModels.VolunteerRequests
{
    public record CreateVolunteerRequest(
        FullNameDto FullName,
        string Email,
        string Description,
        int Experience,
        string PhoneNumber,
       List<SocialNetworkDto> SocialNetworks,
       List<HelpRequisiteDto> HelpRequisites
    ) : IRequest<Result<Guid, ErrorList>>;
}