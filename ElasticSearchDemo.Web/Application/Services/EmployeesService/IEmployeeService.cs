using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Web.Application.Services.Employees
{
    public interface IEmployeeService
    {
        Task<GetEmployeeByIdResponse> GetByIdAsync(GetEmployeeByIdRequest request, CancellationToken cancellationToken);
    }
}