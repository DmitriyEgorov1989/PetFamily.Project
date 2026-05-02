using Microsoft.OpenApi.Models;
using PetFamily.Api.Controllers;
using PetFamily.Api.Middlewares;
using PetFamily.Volunteers.Core.Inject;
using PetFamily.Volunteers.Infrastructure.Adapters.Postgres.WriteDataBase;
using PetFamily.Volunteers.Infrastructure.DependencyInjection;
using Serilog;

namespace PetFamily.Api;

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
        builder.Services.AddVolunteersApplication()
            .AddVolunteersInfrastructure(builder.Configuration);
        //Application           

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PetFamily.Api",
                Version = "v1",
                Description = "API для взаимодействия волонтеров и питомцев"
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. " +
                              "\r\n\r\n Enter 'Bearer'[space] and then your token in the text input below." +
                              "\r\n\r\nExample: \"Bearer token\""
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });


        builder.Services.AddControllers()
            .AddApplicationPart(typeof(VolunteerController).Assembly);
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


        builder.Services.AddDbContext<VolunteersDbContext>();

        builder.Logging.ClearProviders();


        var app = builder.Build();

        app.UseSwagger();

        app.UseSwaggerUI();

        //custom exception handling middleware
        app.UseExceptionHandling();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}