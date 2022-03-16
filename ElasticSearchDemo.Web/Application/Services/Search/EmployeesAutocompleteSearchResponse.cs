using System.Collections.Generic;

namespace ElasticSearchDemo.Web.Application.Services.Search
{
    public class EmployeesSearchAsYouTypeResponse
    {
        public List<SearchAsYouTypeMatch> Results { get; set; } = new List<SearchAsYouTypeMatch>();
    }

    public class SearchAsYouTypeMatch
    {
        public string Text { get; set; }
    }
}