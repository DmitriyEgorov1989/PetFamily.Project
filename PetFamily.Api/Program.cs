using Microsoft.OpenApi.Models;
using PetFamily.Api.Middlewares;
using PetFamily.Core.Application.Inject;
using PetFamily.Infrastructure.Adapters.Inject;
using PetFamily.Infrastructure.Adapters.Postgres.WriteDataBase;
using Serilog;

namespace PetFamily.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //serilog
            Log.Logger = new LoggerConfiguration()
                       .MinimumLevel.Information()
                      .Enrich.FromLogContext()
                      .WriteTo.Console()
                      .WriteTo.Seq("http://localhost:5341")
                      .Enrich.WithThreadName()
                      .CreateLogger();

            builder.Services.AddSerilog(Log.Logger);

            builder.Host.UseSerilog();
            //Infrastructure
            builder.Services.AddInfrastructure(builder.Configuration);
            //Application           
            builder.Services.AddApplication();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PetFamily.Api",
                    Version = "v1",
                    Description = "API для взаимодействия волнтеров и питомцев"
                });
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


            builder.Services.AddDbContext<ApplicationDbContext>();

            builder.Logging.ClearProviders();


            var app = builder.Build();

            app.UseSwagger();

            app.UseSwaggerUI();

            //custom exception handling middleware
            app.UseExceptionHandling();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

    }
}