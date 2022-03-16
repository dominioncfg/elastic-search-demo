using ElasticSearchDemo.DomainModel.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearchDemo.DomainModel.Repositories
{
    public interface IEmployeesRepository
    {
        Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken);

        Task<List<Employee>> GetByIds(IEnumerable<int> ids,CancellationToken cancellationToken);
        Task AddAsync(Employee e,CancellationToken cancellationToken);

        Task AddBatchAsync(IEnumerable<Employee> employees, CancellationToken cancellationToken);
    }
}