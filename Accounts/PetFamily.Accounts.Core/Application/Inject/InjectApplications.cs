using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PetFamily.Accounts.Core.Application.Inject;

public static class InjectApplications
{
    public static void AddAccountsApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => { c.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()); });
        services.AddValidatorsFromAssemblyContaining(typeof(InjectApplications));
    }
}