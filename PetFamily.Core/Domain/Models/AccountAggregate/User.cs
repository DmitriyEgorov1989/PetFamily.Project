using Microsoft.AspNetCore.Identity;

namespace PetFamily.Core.Domain.Models.AccountAggregate
{
    public class User : IdentityUser<Guid>
    {
    }
}