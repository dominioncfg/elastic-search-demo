using ElasticSearchDemo.DomainModel.Repositories;
using ElasticSearchDemo.Infraestructure.Repositories;
using ElasticSearchDemo.SeedProccess.Models.Configuration;
using ElasticSearchDemo.SeedProccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ElasticSearchDemo.SeedProccess.Configuration
{
    public static class CustomServicesConfigurationExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SeedDataConfiguration>(configuration.GetSection(SeedDataConfiguration.SectionName));
            services.AddTransient<EmployeesFileRepository>(services=>
            {
                var options = services.GetRequiredService<IOptions< SeedDataConfiguration>>();
                return new EmployeesFileRepository(options.Value.ImportFileName);
            });
            services.AddTransient<IEmployeesRepository,EntityFrameworkEmployeesRepository>();
            return services;
        }
    }
}
