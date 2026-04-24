using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.MakeMainPhotoPets;

public record MakeMainPhotoPetCommand(
    Guid VolunteerId,
    Guid PetId,
    string PathStorage) : IRequest<Result<string, Error.ErrorList>>;