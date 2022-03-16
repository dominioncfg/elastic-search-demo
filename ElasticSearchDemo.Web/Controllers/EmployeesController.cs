using ElasticSearchDemo.Web.Application.Services;
using ElasticSearchDemo.Web.Application.Services.Employees;
using ElasticSearchDemo.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Web.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            this._employeeService = employeeService;
        }
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var request = new GetEmployeeByIdRequest() { Id = id };
            var viewData = await _employeeService.GetByIdAsync(request, cancellationToken);
            if (viewData == null || viewData.Employee == null)
            {
                return NotFound();
            }
            var viewModel = new EmployeeDetailsViewModel()
            {
                Employee = EmployeeDetailsResponseItem.FromServiceResponse(viewData.Employee)
            };
            return View(viewModel);
        }
    }
}