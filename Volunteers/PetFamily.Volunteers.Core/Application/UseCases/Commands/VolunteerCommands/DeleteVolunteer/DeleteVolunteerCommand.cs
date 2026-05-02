using CSharpFunctionalExtensions;
using MediatR;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.DeleteVolunteer;

public record DeleteVolunteerCommand(Guid VolunteerId)
    : IRequest<UnitResult<ErrorList>>;