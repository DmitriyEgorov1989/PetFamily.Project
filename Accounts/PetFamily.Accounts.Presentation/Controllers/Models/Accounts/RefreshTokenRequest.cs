using PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.RefreshLoginToken;

namespace PetFamily.Accounts.Presentation.Controllers.Models.Accounts
{
    public record class RefreshTokenRequest(string AccessToken, string RefreshToken)
    {
        public RefreshLoginTokenCommand ToCommand() =>
            new RefreshLoginTokenCommand(AccessToken, RefreshToken);
    }
}