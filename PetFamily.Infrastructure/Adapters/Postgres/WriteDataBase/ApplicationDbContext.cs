using Microsoft.EntityFrameworkCore;
using PetFamily.Core.Domain.Models.Species;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using System.Reflection;

namespace PetFamily.Infrastructure.Adapters.Postgres.WriteDataBase
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Species> Species { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}