using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetFamily.Accounts.Core.Domain.Models;

namespace PetFamily.Accounts.Infrastructure.Adapters.Postgres;

public class AccountDbContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public AccountDbContext(DbContextOptions<AccountDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("accounts");

        builder.Entity<IdentityUserRole<Guid>>()
            .ToTable("user_roles");

        builder.ApplyConfigurationsFromAssembly(typeof(AccountDbContext).Assembly);
    }
}