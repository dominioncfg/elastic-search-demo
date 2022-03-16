using ElasticSearchDemo.Infraestructure.EntityFramework;
using ElasticSearchDemo.Web.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElasticSearchDemo.Web.Configuration
{
    public static class CustomEntityFrameworkConfiguration
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new DatabasesConfiguration();
            configuration.GetSection(DatabasesConfiguration.SectionName).Bind(options);
            services.AddDbContext<EmployeesDbContext>(opts => opts.UseSqlServer(options.EmployeesConnectionString));
            return services;
        }
    }
}
