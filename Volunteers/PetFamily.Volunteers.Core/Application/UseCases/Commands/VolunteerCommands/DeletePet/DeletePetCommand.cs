using CSharpFunctionalExtensions;
using MediatR;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.DeletePet;

public record DeletePetCommand(Guid VolunteerId, Guid PetId) : IRequest<UnitResult<ErrorList>>;