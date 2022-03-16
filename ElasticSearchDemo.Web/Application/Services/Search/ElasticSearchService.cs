using ElasticSearchDemo.DomainModel.Models;
using ElasticSearchDemo.DomainModel.Repositories;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Web.Application.Services.Search
{
    public class ElasticSearchService : ISearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly IEmployeesRepository _employeesRepository;

        public ElasticSearchService(
                                        IElasticClient elasticClient,
                                        IEmployeesRepository employeesRepository
                                   )
        {
            _elasticClient = elasticClient;
            _employeesRepository = employeesRepository;
        }

        #region Aggregations       
        private AggregationContainerDescriptor<Employee> GetElasticSearchAggregations()
        {
            AggregationContainerDescriptor<Employee> aggregations = new AggregationContainerDescriptor<Employee>();
            aggregations
                //Designation
                .Terms(SearchServiceConfigurationConstants.DesignationsAggName, t => t
                    .Field(e => e.Designation.Suffix("keyword"))
                    .Size(50)
                    .MinimumDocumentCount(0)
                    .Order(f => f.KeyAscending())
                )
                //Gender
                .Terms(SearchServiceConfigurationConstants.GenderAggName, t => t
                    .Field(e => e.Gender.Suffix("keyword"))
                    .MinimumDocumentCount(0)
                    .Order(f => f.KeyAscending())
                )
                //MaritalStatus
                .Terms(SearchServiceConfigurationConstants.MaritialStatusAggName, t => t
                    .Field(e => e.MaritalStatus.Suffix("keyword"))
                    .MinimumDocumentCount(0)
                    .Order(f => f.KeyAscending())
                )
               //Salary
               .Range(SearchServiceConfigurationConstants.SalaryHistogramAggName, r => r
                    .Field(e => e.Salary)
                    .Ranges(
                        r => r.To(30000),
                        r => r.From(30000).To(45000),
                        r => r.From(45000).To(60000),
                        r => r.From(60000).To(75000),
                        r => r.From(75000).To(90000),
                        r => r.From(90000).To(105000),
                        r => r.From(105000).To(120000),
                        r => r.From(120000).To(135000),
                        r => r.From(135000).To(150000),
                        r => r.From(150000)
                    )
                )
                //Age
                .Range(SearchServiceConfigurationConstants.AgeRangeAggName, r => r
                    .Field(e => e.Age)
                    .Ranges(
                        r => r.From(17).To(30),
                        r => r.From(30).To(45),
                        r => r.From(45).To(60),
                        r => r.From(60)
                    )
                )
                //Date
                .DateRange(SearchServiceConfigurationConstants.DateOfJoiningRangeAggName, dr => dr
                    .Field(e => e.DateOfJoining)
                    .Ranges(SearchServiceConfigurationConstants.FilterDateRanges.ToArray())
                );
            return aggregations;
        }
        private AggregationsResponse ReadAggregationsFromResponse(ISearchResponse<Employee> searchResponse)
        {
            List<AggregationResultBase> designations = new List<AggregationResultBase>();
            var designationsAggregation = searchResponse.Aggregations.Terms(SearchServiceConfigurationConstants.DesignationsAggName);
            if (designationsAggregation != null)
            {
                foreach (var bucket in designationsAggregation.Buckets)
                {
                    designations.Add(new AggregationResultBase()
                    {
                        Name = bucket.Key,
                        Count = bucket.DocCount ?? 0,
                    });
                }
            }

            List<AggregationResultBase> genders = new List<AggregationResultBase>();
            var gendersAggregation = searchResponse.Aggregations.Terms(SearchServiceConfigurationConstants.GenderAggName);
            if (gendersAggregation != null)
            {
                foreach (var bucket in gendersAggregation.Buckets)
                {
                    genders.Add(new AggregationResultBase()
                    {
                        Name = bucket.Key,
                        Count = bucket.DocCount ?? 0,
                    });
                }
            }


            List<AggregationResultBase> maritialStatusTypes = new List<AggregationResultBase>();
            var maritialStatusAggregation = searchResponse.Aggregations.Terms(SearchServiceConfigurationConstants.MaritialStatusAggName);
            if (maritialStatusAggregation != null)
            {
                foreach (var bucket in maritialStatusAggregation.Buckets)
                {
                    maritialStatusTypes.Add(new AggregationResultBase()
                    {
                        Name = bucket.Key,
                        Count = bucket.DocCount ?? 0,
                    });
                }
            }


            List<RangeAggregationResult<int?>> salaryRanges = new List<RangeAggregationResult<int?>>();
            var salaryAggregation = searchResponse.Aggregations.Range(SearchServiceConfigurationConstants.SalaryHistogramAggName);
            if (salaryAggregation != null)
            {
                var buckets = salaryAggregation.Buckets.ToList();
                for (int ind = 0; ind < buckets.Count; ind++)
                {
                    var bucket = buckets[ind];

                    string from = bucket.From?.ToString();
                    if (string.IsNullOrEmpty(from))
                    {
                        from = " <= ";
                    }
                    else if (!bucket.To.HasValue)
                    {
                        from = " >= " + from;
                    }
                    else
                    {
                        from += " - ";
                    }
                    string to = $"{bucket.To}";
                    salaryRanges.Add(new RangeAggregationResult<int?>()
                    {
                        Name = from + to,
                        Count = bucket.DocCount,
                        LowerValue = (int?)bucket.From,
                        UpperValue = (int?)bucket.To,
                    });
                }
            }

            List<RangeAggregationResult<int?>> ageRanges = new List<RangeAggregationResult<int?>>();
            var ageAggregation = searchResponse.Aggregations.Range(SearchServiceConfigurationConstants.AgeRangeAggName);
            if (ageAggregation != null)
            {
                var buckets = ageAggregation.Buckets.ToList();
                for (int ind = 0; ind < buckets.Count; ind++)
                {
                    var bucket = buckets[ind];

                    string from = bucket.From?.ToString();
                    if (string.IsNullOrEmpty(from))
                    {
                        from = " <= ";
                    }
                    else if (!bucket.To.HasValue)
                    {
                        from = " >= " + from;
                    }
                    else
                    {
                        from += " - ";
                    }
                    string to = $"{bucket.To}";
                    ageRanges.Add(new RangeAggregationResult<int?>()
                    {
                        Name = from + to,
                        Count = bucket.DocCount,
                        LowerValue = (int?)bucket.From,
                        UpperValue = (int?)bucket.To,
                    });
                }
            }

            List<RangeAggregationResult<string>> dateOfJoiningRanges = new List<RangeAggregationResult<string>>();
            var dateOfJoiningAggregation = searchResponse.Aggregations.DateRange(SearchServiceConfigurationConstants.DateOfJoiningRangeAggName);
            if (dateOfJoiningAggregation != null)
            {
                var buckets = dateOfJoiningAggregation.Buckets.ToList();
                for (int ind = 0; ind < buckets.Count; ind++)
                {
                    var bucket = buckets[ind];
                    var originalExp = SearchServiceConfigurationConstants.FilterDateRanges.FirstOrDefault(x => x.Key == bucket.Key);
                    var originalFrom = originalExp?.From?.ToString();
                    var originalTo = originalExp?.To?.ToString();
                    if (!string.IsNullOrEmpty(originalFrom))
                    {
                        originalFrom = originalFrom.Replace("now-", "");
                    }
                    if (!string.IsNullOrEmpty(originalTo))
                    {
                        originalTo = originalTo.Replace("now-", "");
                    }

                    dateOfJoiningRanges.Add(new RangeAggregationResult<string>()
                    {
                        Name = bucket.Key,
                        Count = bucket.DocCount,
                        LowerValue = originalFrom,
                        UpperValue = originalTo,
                    });
                }
            }


            AggregationsResponse response = new AggregationsResponse()
            {
                Designations = designations,
                Genders = genders,
                MaritialStatusTypes = maritialStatusTypes,
                SalaryRanges = salaryRanges,
                AgeRanges = ageRanges,
                DateOfJoiningRanges = dateOfJoiningRanges,
            };
            return response;
        }
        #endregion

        #region Bool Query Filter
        private List<QueryContainerDescriptor<Employee>> GetQueryMustContextFilters(SearchFilters filters)
        {
            List<QueryContainerDescriptor<Employee>> queryontextContainerDescriptor = new List<QueryContainerDescriptor<Employee>>();
            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.QueryString))
                {
                    MultiMatchQueryDescriptor<Employee> filter = new MultiMatchQueryDescriptor<Employee>();
                    filter
                        .Fields(f => f
                                .Field(p => p.Interests)
                               .Field(p => p.FirstName)
                               .Field(p => p.LastName)
                               .Field(p => p.Designation)
                               .Field(p => p.Address)
                               )
                        .Query(filters.QueryString)
                        .Fuzziness(Fuzziness.Auto);

                    var query = new QueryContainerDescriptor<Employee>();
                    query.MultiMatch(_ => filter);
                    queryontextContainerDescriptor.Add(query);
                }
            }
            return queryontextContainerDescriptor;
        }
        private List<QueryContainerDescriptor<Employee>> GetQueryFilterContextFilters(SearchFilters filters)
        {
            List<QueryContainerDescriptor<Employee>> filterContextContainerDescriptor = new List<QueryContainerDescriptor<Employee>>();
            filterContextContainerDescriptor = new List<QueryContainerDescriptor<Employee>>();
            if (filters != null)
            {
                if (filters.Designations != null && filters.Designations.Length > 0)
                {
                    var query = new QueryContainerDescriptor<Employee>();
                    query
                                    .Terms(t => t
                                        .Field(model => model.Designation.Suffix("keyword"))
                                        .Terms(filters.Designations.ToArray()
                                    )
                                );
                    filterContextContainerDescriptor.Add(query);
                }

                if (filters.Gender != null && filters.Gender.Length > 0)
                {
                    var query = new QueryContainerDescriptor<Employee>();
                    query
                                    .Terms(t => t
                                        .Field(model => model.Gender.Suffix("keyword"))
                                        .Terms(filters.Gender.ToArray()
                                     )
                                 );
                    filterContextContainerDescriptor.Add(query);
                }

                if (filters.MaritalStatus != null && filters.MaritalStatus.Length > 0)
                {
                    var query = new QueryContainerDescriptor<Employee>();
                    query
                                    .Terms(t => t
                                        .Field(model => model.MaritalStatus.Suffix("keyword"))
                                        .Terms(filters.MaritalStatus.ToArray()
                                    )
                                );
                    filterContextContainerDescriptor.Add(query);
                }

                if (filters.AgeRanges != null && filters.AgeRanges.Count() > 0)
                {
                    List<QueryContainer> nestedQueryPredicates = new List<QueryContainer>();

                    foreach (var ageRange in filters.AgeRanges)
                    {
                        if (ageRange.LowerBound == null && ageRange.UpperBound == null)
                        {
                            continue;
                        }

                        NumericRangeQueryDescriptor<Employee> range = new NumericRangeQueryDescriptor<Employee>();
                        range.Field(x => x.Age);
                        if (ageRange.LowerBound != null)
                        {
                            range.GreaterThanOrEquals(ageRange.LowerBound);
                        }
                        if (ageRange.UpperBound != null)
                        {
                            range.LessThan(ageRange.UpperBound);
                        }
                        var rangeQuery = new QueryContainerDescriptor<Employee>();
                        rangeQuery.Range(_ => range);
                        nestedQueryPredicates.Add(rangeQuery);
                    }

                    BoolQueryDescriptor<Employee> nestedQuery = new BoolQueryDescriptor<Employee>();
                    nestedQuery.Should(nestedQueryPredicates.ToArray());
                    var query = new QueryContainerDescriptor<Employee>();
                    query.Bool(_ => nestedQuery);
                    filterContextContainerDescriptor.Add(query);
                }

                if (filters.SalaryRanges != null && filters.SalaryRanges.Count() > 0)
                {
                    List<QueryContainer> nestedQueryPredicates = new List<QueryContainer>();

                    foreach (var salaryRange in filters.SalaryRanges)
                    {
                        NumericRangeQueryDescriptor<Employee> range = new NumericRangeQueryDescriptor<Employee>();
                        range.Field(x => x.Salary);
                        if (salaryRange.LowerBound != null)
                        {
                            range.GreaterThanOrEquals(salaryRange.LowerBound);
                        }
                        if (salaryRange.UpperBound != null)
                        {
                            range.LessThan(salaryRange.UpperBound);
                        }
                        var rangeQuery = new QueryContainerDescriptor<Employee>();
                        rangeQuery.Range(_ => range);
                        nestedQueryPredicates.Add(rangeQuery);
                    }

                    BoolQueryDescriptor<Employee> nestedQuery = new BoolQueryDescriptor<Employee>();
                    nestedQuery.Should(nestedQueryPredicates.ToArray());
                    var query = new QueryContainerDescriptor<Employee>();
                    query.Bool(_ => nestedQuery);
                    filterContextContainerDescriptor.Add(query);
                }


                if (filters.DateRanges != null && filters.DateRanges.Count() > 0)
                {
                    List<QueryContainer> nestedQueryPredicates = new List<QueryContainer>();

                    foreach (var dateRange in filters.DateRanges)
                    {
                        DateRangeQueryDescriptor<Employee> range = new DateRangeQueryDescriptor<Employee>();
                        range.Field(x => x.DateOfJoining);
                        if (dateRange.LowerBound != null)
                        {
                            range.GreaterThanOrEquals(DateMath.Now.Subtract(dateRange.LowerBound));
                        }
                        if (dateRange.UpperBound != null)
                        {
                            range.LessThan(DateMath.Now.Subtract(dateRange.UpperBound));
                        }
                        var rangeQuery = new QueryContainerDescriptor<Employee>();
                        rangeQuery.DateRange(_ => range);
                        nestedQueryPredicates.Add(rangeQuery);
                    }

                    BoolQueryDescriptor<Employee> nestedQuery = new BoolQueryDescriptor<Employee>();
                    nestedQuery.Should(nestedQueryPredicates.ToArray());
                    var query = new QueryContainerDescriptor<Employee>();
                    query.Bool(_ => nestedQuery);
                    filterContextContainerDescriptor.Add(query);
                }

            }
            return filterContextContainerDescriptor;
        }
        #endregion

        #region Pagination
        private PagingInfo BuildPaginationModel(long totalResults, int page, int pageSize)
        {

            int pages = (int)Math.Ceiling((double)totalResults / (double)pageSize);
            int startElement = (pageSize * (page - 1)) + 1;
            int endElement = startElement + pageSize - 1;

            if (startElement > totalResults)
            {
                startElement = (int)totalResults;
            }

            if (endElement > totalResults)
            {
                endElement = (int)totalResults;
            }
            if (page > pages)
            {
                page = pages;
            }
            var result = new PagingInfo()
            {
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = pages,
                TotalCount = totalResults,
                StartElement = startElement,
                EndElement = endElement,
            };

            return result;
        }
        #endregion

        #region Read From DB
        private async Task<List<EmployeeResponseItem>> ReadFromDataBaseAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            List<EmployeeResponseItem> result = new List<EmployeeResponseItem>();
            var emp = await _employeesRepository.GetByIds(ids, cancellationToken);
            foreach (var item in emp)
            {
                result.Add(EmployeeResponseItem.FromEmployee(item));
            }
            return result;
        }
        #endregion

        public async Task<EmployeesSearchResponse> SearchAsync(EmployeesSearchRequest request, CancellationToken cancellationToken)
        {
            var filters = request.Filters;
            int from = request.PageSize * (request.Page - 1);

            var sendAggregations = GetElasticSearchAggregations();
            var filterContextContainerDescriptor = GetQueryFilterContextFilters(request.Filters);
            var queryontextContainerDescriptor = GetQueryMustContextFilters(request.Filters);

            var searchResponse = await _elasticClient.SearchAsync<Employee>(s => s
               .Size(request.PageSize)
               .From(from)
               .Source(model => model.Includes(f => f.Field(m => m.Id)))
               .Aggregations(_ => sendAggregations)
               .Query(q => q
                    .Bool(boolQuery => boolQuery
                        .Filter(filterContextContainerDescriptor.ToArray())
                        .Must(queryontextContainerDescriptor.ToArray())
                    )
               )
            );
            var querySent = Encoding.UTF8.GetString(searchResponse.ApiCall.RequestBodyInBytes);
            var result = new EmployeesSearchResponse();

            if (searchResponse != null)
            {
                var filtersResponse = ReadAggregationsFromResponse(searchResponse);
                result.Filters = filtersResponse;
                result.PageInfo = BuildPaginationModel(searchResponse.Total, request.Page, request.PageSize);

                if (searchResponse.Documents != null && searchResponse.Documents.Count > 0)
                {
                    var ids = searchResponse.Documents.Select(x => x.Id);
                    var resultItems = await ReadFromDataBaseAsync(ids, cancellationToken);
                    result.Results = resultItems;
                }
            }
            return result;
        }

        public async Task<EmployeesSearchAsYouTypeResponse> SearchAsYouType(EmployeesSearchAsYouTypeRequest request, CancellationToken cancellationToken)
        {
            EmployeesSearchAsYouTypeResponse response = new EmployeesSearchAsYouTypeResponse();

            string query = request.Query;

            if (!string.IsNullOrEmpty(query))
            {
                var searchResponse = await _elasticClient.SearchAsync<Employee>(s => s
                     .Size(10)
                     //.Source(model => model.Includes(f => f.Field(m => m.Id)))
                     .Query(q => q
                          .MultiMatch(multiMatch => multiMatch
                               .Fields(f => f
                                           .Field(p => p.Interests.Suffix("autocomplete"))
                                           .Field(p => p.Interests.Suffix("autocomplete").Suffix("_2gram"))
                                           .Field(p => p.Interests.Suffix("autocomplete").Suffix("_3gram"))
                                           .Field(p => p.Interests.Suffix("autocomplete").Suffix("_4gram"))
                                      )
                               .Type(TextQueryType.BoolPrefix)
                               .Query(query)
                          )
                     )
                  );

                if (searchResponse?.Documents != null)
                {
                    foreach (var doc in searchResponse.Documents)
                    {
                        response.Results.Add(new SearchAsYouTypeMatch() { Text = $"{doc.FirstName} {doc.LastName}" });
                    }
                }
            }

            return response;
        }
    }
}
