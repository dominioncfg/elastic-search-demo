using System.Collections.Generic;

namespace ElasticSearchDemo.Web.Application.Services.Search
{
    public class EmployeesSearchRequest
    {
        public SearchFilters Filters { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchFilters
    {
        public string[] Designations { get; set; }
        public string[] Gender { get; set; }
        public string[] MaritalStatus { get; set; }
        public IEnumerable<RangeFilter<int?>> AgeRanges { get; set; }
        public IEnumerable<RangeFilter<int?>> SalaryRanges { get; set; }
        public IEnumerable<RangeFilter<string>> DateRanges { get; set; }
        public string QueryString { get; set; }
    }

    public class RangeFilter<T> 
    {
        public T LowerBound { get; set; }
        public T UpperBound { get; set; }
    }
}