using FluentValidation;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Extensions.Validations;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Queries.GetPermissionsByRoleId;

public class GetPermissionsByRoleIdValidator : AbstractValidator<GetPermissionsByRoleIdQuery>
{
    public GetPermissionsByRoleIdValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithError(GeneralErrors.ValueIsInvalid("RoleId"));
    }
}