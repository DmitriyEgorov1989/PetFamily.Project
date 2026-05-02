using Microsoft.EntityFrameworkCore;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate;
using System.Reflection;

namespace PetFamily.Volunteers.Infrastructure.Adapters.Postgres.WriteDataBase;

public class VolunteersDbContext : DbContext
{
    public VolunteersDbContext(DbContextOptions<VolunteersDbContext> options)
        : base(options)
    {
    }
    public DbSet<Volunteer> Volunteers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}