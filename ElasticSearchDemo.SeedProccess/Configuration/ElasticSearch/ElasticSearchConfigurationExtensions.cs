using ElasticSearchDemo.SeedProccess.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace ElasticSearchDemo.SeedProccess.Configuration
{
    public static class ElasticSearchConfigurationExtensions
    {
        public static IServiceCollection AddCustomElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ElasticSearchConfiguration>(configuration.GetSection(ElasticSearchConfiguration.SectionName));
            var elasticSearchOptions = new ElasticSearchConfiguration();
            configuration.GetSection(ElasticSearchConfiguration.SectionName).Bind(elasticSearchOptions);

            services.AddSingleton<IElasticClient, ElasticClient>(services =>
            {
                var settings = new ConnectionSettings(new System.Uri(elasticSearchOptions.ServerEndpoint))
                 .DefaultIndex(elasticSearchOptions.EmployeesIndexName);
                var elasticClient = new ElasticClient(settings);
                return elasticClient;
            });

            return services;
        }
    }
}
