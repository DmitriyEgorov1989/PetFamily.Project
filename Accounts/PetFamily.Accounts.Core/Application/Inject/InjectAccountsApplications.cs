using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PetFamily.Accounts.Core.Application.Inject;

public static class InjectAccountsApplications
{
    public static IServiceCollection AddAccountsApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => { c.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()); });
        services.AddValidatorsFromAssemblyContaining(typeof(InjectAccountsApplications));
        return services;
    }
}