using CSharpFunctionalExtensions;
using MediatR;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Queries.GetVolunteerById;

public record GetVolunteerByIdQuery(Guid VolunteerId) :
    IRequest<Result<GetVolunteerByIdResponse, ErrorList>>;