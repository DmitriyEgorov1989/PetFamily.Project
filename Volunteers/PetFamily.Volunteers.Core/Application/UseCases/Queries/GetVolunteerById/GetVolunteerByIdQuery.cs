using CSharpFunctionalExtensions;
using MediatR;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Queries.GetVolunteerById;

public record GetVolunteerByIdQuery(Guid VolunteerId) :
    IRequest<Result<GetVolunteerByIdResponse, ErrorList>>;