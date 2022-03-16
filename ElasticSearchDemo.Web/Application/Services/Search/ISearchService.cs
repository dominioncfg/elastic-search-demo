using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Web.Application.Services.Search
{
    public interface ISearchService
    {
        Task<EmployeesSearchResponse> SearchAsync(EmployeesSearchRequest request, CancellationToken cancellationToken);
        Task<EmployeesSearchAsYouTypeResponse> SearchAsYouType(EmployeesSearchAsYouTypeRequest request, CancellationToken cancellationToken);
        
    }
}