using Microsoft.Extensions.DependencyInjection;
using OmniSuite.Application.Pipeline;
using System.Reflection;

namespace OmniSuite.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); // FluentValidation
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); // Pipeline


            return services;
        }
    }
}
