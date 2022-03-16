using ElasticSearchDemo.Web.Application.Services.Search;
using System.Collections.Generic;

namespace ElasticSearchDemo.Web.ViewModels
{
    public class SearchViewModel
    {
        public List<EmployeeResponseItem> Results { get; set; }
        public PagingInfoViewModel PageInfo { get; set; }
        public SearchFiltersViewModel Filters { get; set; }
        public QueryFilterViewModel SearchBoxQueryViewModel { get; set; }
        public IEnumerable<AggregationBaseViewModel> ActiveFilters { get; set; }
    }
    public class QueryFilterViewModel
    {
        public string Query { get; set; }
        public List<(string, string)> ActiveFilters { get; set; }
    }

    public class PagingInfoViewModel
    {
        public long TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public long StartElement { get; set; }
        public long EndElement { get; set; }
        public string NextPageUrl { get; set; }
        public string PrevPageUrl { get; set; }
    }
}
