using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Ports.DataBaseForRead;
using Serilog;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllPetsWithPaginationAndFilter;

public class GetAllPetsWithPaginationAndFilterHandler :
    IRequestHandler<GetAllPetsWithPaginationAndFilterQuery,
        Result<GetAllPetsWithPaginationAndFilterResponse, ErrorList>>
{
    private readonly ILogger _logger;
    private readonly IPetsReadRepository _petsReadRepository;
    private readonly IValidator<GetAllPetsWithPaginationAndFilterQuery> _validator;

    public GetAllPetsWithPaginationAndFilterHandler(
        IPetsReadRepository petsReadRepository,
        IValidator<GetAllPetsWithPaginationAndFilterQuery> validator,
        ILogger logger)
    {
        _petsReadRepository = petsReadRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GetAllPetsWithPaginationAndFilterResponse, ErrorList>> Handle(
        GetAllPetsWithPaginationAndFilterQuery query, CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(query, cancellationToken);

        if (!resultValidation.IsValid) return resultValidation.ToValidationErrorResponse(query);

        var resultGetAllPets = await _petsReadRepository
            .GetAllWithPaginationAndFiltersAsync(
                query.PaginationData,
                query.FilterByData,
                query.SortByData,
                cancellationToken);

        if (resultGetAllPets.IsFailure)
        {
            _logger.Information("Failed to get all pets with pagination and filter.");
            return (ErrorList)resultGetAllPets.Error;
        }

        return new GetAllPetsWithPaginationAndFilterResponse(resultGetAllPets.Value);
    }
}