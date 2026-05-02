using PetFamily.Accounts.Core.Domain.Models.AccountAggregate;

namespace PetFamily.Accounts.Core.Ports;

public interface ITokenProvider
{
    string GenerateAccessToken(User user);
}