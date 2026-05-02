using PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.RegistrationUser;

namespace PetFamily.Accounts.Presentation.Controllers.Models.Accounts;

public record RegistrationUserRequest(string Email, string UserName, string Password)
{
    public RegistrationUserCommand ToCommand()
    {
        return new RegistrationUserCommand(Email, UserName, Password);
    }
}