using ElasticSearchDemo.DomainModel.Models;
using ElasticSearchDemo.DomainModel.Repositories;
using ElasticSearchDemo.SeedProccess.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearchDemo.SeedProccess.Repositories
{
    public class EmployeesFileRepository : IEmployeesRepository
    {
        public readonly string _fileName;
        public EmployeesFileRepository(string fileName)
        {
            _fileName = fileName;
        }

        public Task AddAsync(Employee e, CancellationToken cancellationToken) => throw new NotImplementedException();

        public Task AddBatchAsync(IEnumerable<Employee> e, CancellationToken cancellationToken) => throw new NotImplementedException();

        public Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken)
        {
            string rawEntities = File.ReadAllText(_fileName);

            List<EmployeeDTO> rawEmployees = JsonSerializer.Deserialize<List<EmployeeDTO>>(rawEntities);

            var employeesModels = rawEmployees.Select((x, i) => new Employee()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Designation = x.Designation,
                Salary = x.Salary,
                DateOfJoining = x.DateOfJoining.Date,
                Address = x.Address,
                Gender = x.Gender,
                Age = x.Age,
                MaritalStatus = x.MaritalStatus,
                Interests = x.Interests,
            }).ToList();
            return Task.FromResult(employeesModels);
        }

        public Task<List<Employee>> GetByIds(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}