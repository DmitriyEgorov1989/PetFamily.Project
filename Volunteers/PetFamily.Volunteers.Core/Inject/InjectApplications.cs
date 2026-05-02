using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PetFamily.Volunteers.Core.Inject;

public static class InjectApplications
{
    public static IServiceCollection AddVolunteersApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => { c.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()); });
        services.AddValidatorsFromAssemblyContaining(typeof(InjectApplications));
        return services;
    }
}