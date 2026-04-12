using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Domain.Models.Species;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using System.Reflection;

namespace PetFamily.Infrastructure.Adapters.Postgres
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Species> Species { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("Postgres");
            optionsBuilder.UseNpgsql(connectionString);
            optionsBuilder.UseCamelCaseNamingConvention();
            optionsBuilder.UseLoggerFactory(CreateLoggreFactory());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private ILoggerFactory CreateLoggreFactory() =>
            LoggerFactory.Create(builder => { builder.AddConsole(); });
    }
}