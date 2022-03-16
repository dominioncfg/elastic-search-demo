using ElasticSearchDemo.DomainModel.Models;
using Microsoft.EntityFrameworkCore;
using EntityConfiguration = ElasticSearchDemo.Infraestructure.EntityFramework.Configuration;

namespace ElasticSearchDemo.Infraestructure.EntityFramework
{
    public class EmployeesDbContext : DbContext
    {
        public virtual DbSet<Employee> Employees { get; set; }
        public EmployeesDbContext(DbContextOptions<EmployeesDbContext> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EntityConfiguration.EmployeeEntityConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
