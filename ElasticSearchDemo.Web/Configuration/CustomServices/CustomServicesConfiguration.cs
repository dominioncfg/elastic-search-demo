using ElasticSearchDemo.DomainModel.Repositories;
using ElasticSearchDemo.Infraestructure.Repositories;
using ElasticSearchDemo.Web.Application.Services.Employees;
using ElasticSearchDemo.Web.Application.Services.Search;
using Microsoft.Extensions.DependencyInjection;

namespace ElasticSearchDemo.Web.Configuration
{
    public static class CustomServicesConfiguration
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IEmployeesRepository, EntityFrameworkEmployeesRepository>();
            services.AddTransient<ISearchService, ElasticSearchService>();
            services.AddTransient<IEmployeeService, EmployeeService>();

            return services;
        }
    }
}