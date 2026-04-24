using CSharpFunctionalExtensions;
using MediatR;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.DeletePet;

public record DeletePetCommand(Guid VolunteerId, Guid PetId) : IRequest<UnitResult<ErrorList>>;