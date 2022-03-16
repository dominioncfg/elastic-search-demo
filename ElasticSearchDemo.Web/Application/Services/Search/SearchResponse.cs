using ElasticSearchDemo.DomainModel.Models;
using System;
using System.Collections.Generic;

namespace ElasticSearchDemo.Web.Application.Services.Search
{
    public class EmployeesSearchResponse
    {
        public List<EmployeeResponseItem> Results { get; set; } = new List<EmployeeResponseItem>();
        public PagingInfo PageInfo { get; set; }

        public AggregationsResponse Filters { get; set; }
    }

    public class PagingInfo
    {
        public long TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public long StartElement { get; set; }
        public long EndElement { get; set; }
    }
    public class EmployeeResponseItem
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

        public static EmployeeResponseItem FromEmployee(Employee e)
        {
            return new EmployeeResponseItem()
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