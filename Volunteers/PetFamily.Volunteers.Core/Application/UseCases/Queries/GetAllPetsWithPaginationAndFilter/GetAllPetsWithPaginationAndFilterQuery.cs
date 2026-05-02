using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllPetsWithPaginationAndFilter;

public record GetAllPetsWithPaginationAndFilterQuery(
    PaginationData PaginationData,
    FilterByData? FilterByData,
    SortByData? SortByData)
    : IRequest<Result<GetAllPetsWithPaginationAndFilterResponse, ErrorList>>;