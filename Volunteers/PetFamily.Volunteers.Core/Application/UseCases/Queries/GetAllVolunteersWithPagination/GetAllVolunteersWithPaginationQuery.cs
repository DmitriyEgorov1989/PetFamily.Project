using CSharpFunctionalExtensions;
using MediatR;
using static PetFamily.SharedKernel.Errors.Error;


namespace PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllVolunteersWithPagination;

public sealed record GetAllVolunteersWithPaginationQuery(int PageNumber, int PageSize) :
    IRequest<Result<GetAllVolunteersWithPaginationResponse, ErrorList>>;