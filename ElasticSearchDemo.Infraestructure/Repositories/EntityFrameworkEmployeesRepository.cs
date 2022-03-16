using ElasticSearchDemo.DomainModel.Models;
using ElasticSearchDemo.DomainModel.Repositories;
using ElasticSearchDemo.Infraestructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Infraestructure.Repositories
{
    public class EntityFrameworkEmployeesRepository : IEmployeesRepository
    {
        private readonly EmployeesDbContext _context;

        public EntityFrameworkEmployeesRepository(EmployeesDbContext context)
        {
            this._context = context;
        }

        public async Task AddAsync(Employee e,CancellationToken cancellationToken)
        {
            await _context.AddAsync(e, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AddBatchAsync(IEnumerable<Employee> employees, CancellationToken cancellationToken)
        {
            await _context.AddRangeAsync(employees, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Employees.ToListAsync(cancellationToken);
        }

        public async Task<List<Employee>> GetByIds(IEnumerable<int> ids,CancellationToken cancellationToken)
        {
            return await _context.Employees.Where(x=>ids.Contains(x.Id)).ToListAsync(cancellationToken);
        }
    }
}