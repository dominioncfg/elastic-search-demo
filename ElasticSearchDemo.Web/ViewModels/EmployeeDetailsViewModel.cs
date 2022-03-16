using ElasticSearchDemo.Web.Application.Services.Employees;
using System;

namespace ElasticSearchDemo.Web.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public EmployeeDetailsResponseItem Employee { get; set; }
    }

    public class EmployeeDetailsResponseItem
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public decimal Salary { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string MaritalStatus { get; set; }
        public string Interests { get; set; }

        public static EmployeeDetailsResponseItem FromServiceResponse(GetByIdEmployeeResponseItem e)
        {
            return new EmployeeDetailsResponseItem()
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Designation = e.Designation,
                Salary = e.Salary,
                DateOfJoining = e.DateOfJoining,
                Address = e.Address,
                Gender = e.Gender,
                Age = e.Age,
                MaritalStatus = e.MaritalStatus,
                Interests = e.Interests,
            };
        }
    }
}