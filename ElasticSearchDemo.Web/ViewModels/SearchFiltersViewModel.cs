using System.Collections.Generic;

namespace ElasticSearchDemo.Web.ViewModels
{
    public class SearchFiltersViewModel
    {
        public IEnumerable<AggregationBaseViewModel> Designations { get; set; }
        public IEnumerable<AggregationBaseViewModel> Genders { get; set; }
        public IEnumerable<AggregationBaseViewModel> MaritialStatusTypes { get; set; }
        public IEnumerable<AggregationBaseViewModel> SalaryRanges { get; set; }
        public IEnumerable<AggregationBaseViewModel> AgeRanges { get; set; }
        public IEnumerable<AggregationBaseViewModel> DateOfJoiningRanges { get; set; }
    }

    public class AggregationBaseViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public long Count { get; set; }
        public string GoUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
