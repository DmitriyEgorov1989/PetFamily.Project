using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Application.UseCases.CommonDto;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.Queries.GetAllPetsWithPaginationAndFilter;

public record GetAllPetsWithPaginationAndFilterQuery(
    PaginationData PaginationData,
    FilterByData? FilterByData,
    SortByData? SortByData)
    : IRequest<Result<GetAllPetsWithPaginationAndFilterResponse, ErrorList>>;