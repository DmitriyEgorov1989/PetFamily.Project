using PetFamily.Accounts.Core.Application.UseCases.AccountManager.Commands.RegistrationUser;
using PetFamily.SharedKernel.DomainModels.VO;

namespace PetFamily.Accounts.Presentation.Controllers.Models.Accounts;

public record RegistrationUserRequest(
    string Email,
    string UserName,
    string FirstName,
    string MiddleName,
    string LastName,
    string PhoneNumber,
    string Password)
{
    public RegistrationUserCommand ToCommand()
    {
        return new RegistrationUserCommand(
            Email,
            UserName,
            FullName.Create(FirstName, MiddleName, LastName).Value,
            PhoneNumber,
            Password);
    }
}