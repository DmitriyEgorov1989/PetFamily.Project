using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.SharedKernel.Errors;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.MakeMainPhotoPets;

public record MakeMainPhotoPetCommand(
    Guid VolunteerId,
    Guid PetId,
    string PathStorage) : IRequest<Result<string, Error.ErrorList>>;