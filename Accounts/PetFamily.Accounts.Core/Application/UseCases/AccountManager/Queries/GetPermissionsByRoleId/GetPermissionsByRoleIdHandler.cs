using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.Accounts.Core.Ports;
using PetFamily.SharedKernel.Extensions.Validations;
using Serilog;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Queries.GetPermissionsByRoleId;

public class GetPermissionsByRoleIdHandler :
    IRequestHandler<GetPermissionsByRoleIdQuery, Result<GetPermissionsByRoleIdResponse, ErrorList>>
{
    private readonly IReadDataProvider _readDataProvider;
    private IValidator<GetPermissionsByRoleIdQuery> _validator;
    private readonly ILogger _logger;

    public GetPermissionsByRoleIdHandler(
        IReadDataProvider readDataProvider,
        IValidator<GetPermissionsByRoleIdQuery> validator,
        ILogger logger)
    {
        _readDataProvider = readDataProvider;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GetPermissionsByRoleIdResponse, ErrorList>> Handle(
        GetPermissionsByRoleIdQuery query,
        CancellationToken cancellationToken)
    {
        var resultValidation = await _validator.ValidateAsync(query, cancellationToken);

        if (!resultValidation.IsValid)
            return resultValidation.ToValidationErrorResponse(query);

        var resultGetPermissionsName =
            await _readDataProvider.GetAllPermissionsByRoleIdAsync(query.Id, cancellationToken);

        if (resultGetPermissionsName.IsFailure)
            return (ErrorList)resultGetPermissionsName.Error;

        return new GetPermissionsByRoleIdResponse(resultGetPermissionsName.Value);
    }
}