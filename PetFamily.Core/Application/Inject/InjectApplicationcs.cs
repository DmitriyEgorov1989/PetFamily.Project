using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PetFamily.Core.Application.Inject
{
    public static class InjectApplicationcs
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(c =>
            {
                c.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
            });
            services.AddValidatorsFromAssemblyContaining(typeof(InjectApplicationcs));

        }
    }
}