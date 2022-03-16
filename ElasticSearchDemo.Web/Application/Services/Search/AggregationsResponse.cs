using System.Collections.Generic;

namespace ElasticSearchDemo.Web.Application.Services.Search
{
    public class AggregationResultBase
    {
        public string Name { get; set; }
        public long Count { get; set; }
    }

    public class RangeAggregationResult<T>: AggregationResultBase 
    {
       public T LowerValue { get; set; }
       public T UpperValue { get; set; }
    }

    public class AggregationsResponse
    {
        public IEnumerable<AggregationResultBase> Designations { get; set; }
        public IEnumerable<AggregationResultBase> Genders { get; set; }
        public IEnumerable<AggregationResultBase> MaritialStatusTypes { get; set; }
        public IEnumerable<RangeAggregationResult<int?>> SalaryRanges { get; set; }
        public IEnumerable<RangeAggregationResult<int?>> AgeRanges { get; set; }
        public IEnumerable<RangeAggregationResult<string>> DateOfJoiningRanges { get; set; }
    }
}
