using ElasticSearchDemo.DomainModel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElasticSearchDemo.Infraestructure.EntityFramework.Configuration
{
    class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable(ConfigurationConstants.Tables.Employees, ConfigurationConstants.Schemas.Core);
            builder.HasKey(e => e.Id);
            builder.Property(m => m.Id).ValueGeneratedNever();
            builder.Property(e => e.Salary).HasColumnType("decimal(15,2)");
        }
    }
}
