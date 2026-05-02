using CSharpFunctionalExtensions;
using MediatR;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.ChangeStatusPet;

public record ChangeStatusPetCommand(Guid VolunteerId, Guid PetId, int HelpStatus)
    : IRequest<UnitResult<ErrorList>>;