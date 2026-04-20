using CSharpFunctionalExtensions;
using MediatR;
using static Primitives.Error;


namespace PetFamily.Core.Application.UseCases.Queries.GetAllVolunteersWithPagination;

public sealed record GetAllVolunteersWithPaginationQuery(int PageNumber, int PageSize) :
    IRequest<Result<GetAllVolunteersWithPaginationResponse, ErrorList>>;