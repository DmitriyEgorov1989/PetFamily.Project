using PetFamily.Core.Domain.Models.AccountAggregate;

namespace PetFamily.Core.Ports
{
    public interface ITokenProvider
    {
        string GenerateAccessToken(User user);
    }
}