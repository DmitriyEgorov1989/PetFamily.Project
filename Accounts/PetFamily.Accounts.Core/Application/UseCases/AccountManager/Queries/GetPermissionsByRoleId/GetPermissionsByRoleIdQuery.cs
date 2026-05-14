using CSharpFunctionalExtensions;
using MediatR;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Queries.GetPermissionsByRoleId;

public record GetPermissionsByRoleIdQuery(Guid Id) :
    IRequest<Result<GetPermissionsByRoleIdResponse, ErrorList>>;