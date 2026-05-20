using FluentValidation;

namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.RefreshLoginToken
{
    public class RefreshLoginTokenValidator :
         AbstractValidator<RefreshLoginTokenCommand>
    {
        public RefreshLoginTokenValidator()
        {
        }
    }
}
