using PetFamily.Accounts.Core.Domain.Models;

namespace PetFamily.Accounts.Core.Ports;

public interface ITokenProvider
{
    string GenerateAccessToken(User user);
}