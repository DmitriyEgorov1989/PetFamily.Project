using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Contracts.Controllers.Models.Volunteers.WriteModels.VolunteerRequests;

public record CreateVolunteerRequest(
    FullNameDto FullName,
    string Email,
    string Description,
    int Experience,
    string PhoneNumber,
    List<SocialNetworkDto> SocialNetworks,
    List<HelpRequisiteDto> HelpRequisites
) : IRequest<Result<Guid, ErrorList>>;