using ElasticSearchDemo.SeedProccess.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElasticSearchDemo.SeedProccess
{
    class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomDbContextAsync(_configuration);
            services.AddCustomElasticSearch(_configuration);
            services.AddCustomServices(_configuration);
        }        
    }
}
