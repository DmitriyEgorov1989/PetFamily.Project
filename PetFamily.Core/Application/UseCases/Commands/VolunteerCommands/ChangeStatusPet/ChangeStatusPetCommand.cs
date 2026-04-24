using CSharpFunctionalExtensions;
using MediatR;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.ChangeStatusPet;

public record ChangeStatusPetCommand(Guid VolunteerId, Guid PetId, int HelpStatus)
    : IRequest<UnitResult<ErrorList>>;