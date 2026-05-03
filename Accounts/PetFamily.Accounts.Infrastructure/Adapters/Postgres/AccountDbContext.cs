using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetFamily.Accounts.Core.Domain.Models.AccountAggregate;

namespace PetFamily.Accounts.Infrastructure.Adapters.Postgres;

public class AccountDbContext : IdentityDbContext<User, Role, Guid>
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
        builder.HasDefaultSchema("accounts");
        builder.Entity<User>()
            .ToTable("users");

        builder.Entity<Role>()
            .ToTable("roles");

        builder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("user_claims");

        builder.Entity<IdentityUserToken<Guid>>()
            .ToTable("user_tokens");

        builder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("user_logins");

        builder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("role_claims");

        builder.Entity<IdentityUserRole<Guid>>()
            .ToTable("user_roles");
    }
}