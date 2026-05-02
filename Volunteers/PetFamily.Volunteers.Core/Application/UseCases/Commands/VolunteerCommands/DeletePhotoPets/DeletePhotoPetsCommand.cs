using CSharpFunctionalExtensions;
using MediatR;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.DeletePhotoPets;

public record DeletePhotoPetsCommand(Guid VolunteerId, Guid PetId, string FileName)
    : IRequest<Result<string, ErrorList>>;