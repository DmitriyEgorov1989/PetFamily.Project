using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Ports.DataBaseForRead;
using Serilog;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllVolunteersWithPagination;

public class GetAllVolunteersWithPaginationHandler : IRequestHandler<GetAllVolunteersWithPaginationQuery,
    Result<GetAllVolunteersWithPaginationResponse, ErrorList>>
{
    private readonly ILogger _logger;
    private readonly IReadVollunteersRepository _readRepository;
    private readonly IValidator<GetAllVolunteersWithPaginationQuery> _validator;

    public GetAllVolunteersWithPaginationHandler(
        IReadVollunteersRepository readRepository,
        ILogger logger, IValidator<GetAllVolunteersWithPaginationQuery> validator)
    {
        _readRepository = readRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<GetAllVolunteersWithPaginationResponse, ErrorList>> Handle
        (GetAllVolunteersWithPaginationQuery query, CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(query, cancellationToken);

        if (!resultValidation.IsValid) return resultValidation.ToValidationErrorResponse(query);
        var resultGetVolunteers =
            await _readRepository.GetAllVolunteersWithPaginationAsync(
                query.PageNumber, query.PageSize, cancellationToken);
        if (resultGetVolunteers.IsFailure)
            return (ErrorList)resultGetVolunteers.Error;

        var listVolunteersDto = resultGetVolunteers.Value.ToList();

        return new GetAllVolunteersWithPaginationResponse(listVolunteersDto);
    }
}