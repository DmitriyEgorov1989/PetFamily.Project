using CSharpFunctionalExtensions;
using MediatR;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.DeletePhotoPets;

public record DeletePhotoPetsCommand(Guid VolunteerId, Guid PetId, string FileName)
    : IRequest<Result<string, ErrorList>>;