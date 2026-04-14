using CSharpFunctionalExtensions;
using MediatR;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.DeleteVolunteer;

public record DeleteVolunteerCommand(Guid VolunteerId)
    : IRequest<UnitResult<ErrorList>>;