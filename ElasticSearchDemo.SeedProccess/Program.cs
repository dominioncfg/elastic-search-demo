using ElasticSearchDemo.Infraestructure.EntityFramework;
using ElasticSearchDemo.SeedProccess.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace ElasticSearchDemo.SeedProccess
{
    class Program
    {
        static async Task Main()
        {
            IConfigurationRoot configRoot = new ConfigurationBuilder()
                                               .AddJsonFile("appsettings.json")
                                               .AddUserSecrets<Program>()
                                               .Build();

            Startup startup = new Startup(configRoot);
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EmployeesDbContext>();
                dbContext.Database.EnsureCreated();

            }

            IndexManager manager  = ActivatorUtilities.GetServiceOrCreateInstance<IndexManager>(serviceProvider);
            await manager.RunAsync();          
        }
    }
}
