using ElasticSearchDemo.DomainModel.Models;
using System;

namespace ElasticSearchDemo.Web.Application.Services.Employees
{
    public class GetEmployeeByIdResponse
    {
        public GetByIdEmployeeResponseItem Employee { get; set; }
    }
    public class GetByIdEmployeeResponseItem
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

        public static GetByIdEmployeeResponseItem FromEmployee(Employee e)
        {
            return new GetByIdEmployeeResponseItem()
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