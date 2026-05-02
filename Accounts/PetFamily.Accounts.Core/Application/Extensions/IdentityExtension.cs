using Microsoft.AspNetCore.Identity;
using PetFamily.SharedKernel.Errors;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Accounts.Core.Application.Extensions;

public static class IdentityExtension
{
    public static ErrorList ToErrorList(this IdentityResult identityResult)
    {
        if (identityResult.Errors.Any())
        {
            var errors = identityResult.Errors.Select(e =>
                GeneralErrors.IdentityUser(e.Code, e.Description)).ToList();
            return new ErrorList(errors);
        }

        return new ErrorList([]);
    }
}