using ElasticSearchDemo.Infraestructure.EntityFramework;
using ElasticSearchDemo.SeedProccess.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElasticSearchDemo.SeedProccess.Configuration
{
    public static class CustomEntityFrameworkConfiguration
    {
        public static IServiceCollection AddCustomDbContextAsync(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new DatabasesConfiguration();
            configuration.GetSection(DatabasesConfiguration.SectionName).Bind(options);
            services.AddDbContext<EmployeesDbContext>(opts => opts.UseSqlServer(options.EmployeesConnectionString));
            return services;
        }
    }
}
