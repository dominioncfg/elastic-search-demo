using ElasticSearchDemo.DomainModel.Models;
using Nest;
using System.Collections.Generic;

namespace ElasticSearchDemo.Web.Application.Services.Search
{
    public static class SearchServiceConfigurationConstants
    {
        public const string DesignationsAggName = nameof(Employee.Designation);
        public const string GenderAggName = nameof(Employee.Gender);
        public const string MaritialStatusAggName = nameof(Employee.MaritalStatus);
        public const string SalaryHistogramAggName = nameof(Employee.Salary);
        public const string AgeRangeAggName = nameof(Employee.Age);
        public const string DateOfJoiningRangeAggName = nameof(Employee.DateOfJoining);
        public static readonly List<IDateRangeExpression> FilterDateRanges = new List<IDateRangeExpression>()
        {
            (new  DateRangeExpressionDescriptor()).From("now-6M").Key("Less than 6 Month"),
            (new  DateRangeExpressionDescriptor()).From("now-1y").To("now-6M").Key("Less than 1 Year"),
            (new  DateRangeExpressionDescriptor()).From("now-3y").To("now-1y").Key("Less than 3 Year"),
            (new  DateRangeExpressionDescriptor()).From("now-5y").To("now-3y").Key("Less than 5 Year"),
            (new  DateRangeExpressionDescriptor()).To("now-5y").Key("More than 5 Year"),
        };
    }
}