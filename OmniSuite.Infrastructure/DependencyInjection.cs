using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OmniSuite.Domain.Interfaces;
using OmniSuite.Infrastructure.Services;
using OmniSuite.Infrastructure.Services.MFAService;
using OmniSuite.Persistence;

namespace OmniSuite.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMfaService, MfaServiceImplementation>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    config.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 43))
                )
            );

            return services;
        }
    }
}
