using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.UpdateSocialNetwork;

public record UpdateSocialNetworkCommand(Guid VolunteerId, List<SocialNetworkDto> SocialNetworks)
    : IRequest<UnitResult<ErrorList>>;