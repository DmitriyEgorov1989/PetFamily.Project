using Microsoft.AspNetCore.Identity;

namespace PetFamily.Accounts.Core.Domain.Models.AccountAggregate;

public class User : IdentityUser<Guid>
{
}