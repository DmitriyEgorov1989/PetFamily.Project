using CSharpFunctionalExtensions;
using MediatR;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllPetsByVolunteerId
{
    public record GetAllPetsByVolunteerIdQuery(Guid VolunteerId) :
        IRequest<Result<GetAllPetsByVolunteerIdResponse, ErrorList>>;
}