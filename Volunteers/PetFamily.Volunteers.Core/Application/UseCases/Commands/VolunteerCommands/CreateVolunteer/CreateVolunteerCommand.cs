using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Application.UseCases.CommonDto;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.CreateVolunteer;

public record CreateVolunteerCommand(
    FullNameDto FullName,
    string Email,
    string Description,
    int Experience,
    string PhoneNumber,
    List<SocialNetworkDto> SocialNetworks,
    List<HelpRequisiteDto> HelpRequisites
) : IRequest<Result<Guid, ErrorList>>;