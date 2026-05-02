using PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.LoginUser;

namespace PetFamily.Accounts.Presentation.Controllers.Models.Accounts;

public record LoginUserRequest(string Email, string Password)
{
    public LoginUserCommand ToCommand()
    {
        return new LoginUserCommand(Email, Password);
    }
}