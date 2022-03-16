using ElasticSearchDemo.DomainModel.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Web.Application.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeesRepository _employeesRepository;

        public EmployeeService(
                                IEmployeesRepository employeesRepository
                              )
        {
            _employeesRepository = employeesRepository;
        }

        public async Task<GetEmployeeByIdResponse> GetByIdAsync(GetEmployeeByIdRequest request, CancellationToken cancellationToken)
        {
            var model = (await _employeesRepository.GetByIds(new int[] { request.Id }, cancellationToken))?.FirstOrDefault();
            GetEmployeeByIdResponse response = new GetEmployeeByIdResponse()
            {
                Employee = GetByIdEmployeeResponseItem.FromEmployee(model),
            };
            return response;
        }
    }
}