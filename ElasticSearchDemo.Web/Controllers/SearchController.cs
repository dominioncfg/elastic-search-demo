using ElasticSearchDemo.Web.Application.Services.Search;
using ElasticSearchDemo.Web.Infra.Helpers;
using ElasticSearchDemo.Web.ViewModels;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        private static (int?, int?) ParseIntRangeFromQueryString(string range)
        {
            int? lower = null;
            int? upper = null;

            if (!string.IsNullOrEmpty(range))
            {
                string[] limits = range.Split("-");
                if (limits.Length >= 1)
                {
                    if (limits[0] != "x")
                    {
                        bool parsed = int.TryParse(limits[0], out int l);
                        if (parsed)
                        {
                            lower = l;
                        }
                    }

                }
                if (limits.Length >= 2)
                {
                    if (limits[1] != "x")
                    {
                        bool parsed = int.TryParse(limits[1], out int u);
                        if (parsed)
                        {
                            upper = u;
                        }
                    }
                }
            }
            if (lower != null && upper != null)
            {
                int l = System.Math.Min(lower.Value, upper.Value);
                int u = System.Math.Max(lower.Value, upper.Value);

                lower = l;
                upper = u;
            }
            return (lower, upper);
        }

        private static (string, string) ParseDateMathRange(string range)
        {
            string lower = null;
            string upper = null;

            if (!string.IsNullOrEmpty(range))
            {
                string[] limits = range.Split("-");
                if (limits.Length >= 1)
                {
                    if (limits[0] != "x")
                    {
                        lower = limits[0];
                    }

                }
                if (limits.Length >= 2)
                {
                    if (limits[1] != "x")
                    {
                        upper = limits[1];
                    }
                }
            }
            return (lower, upper);
        }

        [HttpGet]
        public async Task<IActionResult> Search(
                                                [FromQuery(Name = "q")] string query,
                                                [FromQuery(Name = "designation")] string[] designations,
                                                [FromQuery(Name = "gender")] string[] genders,
                                                [FromQuery(Name = "maritial-status")] string[] maritialStatuses,
                                                [FromQuery(Name = "age")] string[] agesRanges,
                                                [FromQuery(Name = "salary")] string[] salaryRanges,
                                                [FromQuery(Name = "joining")] string[] strJoins,
                                                CancellationToken cancellationToken,
                                                [FromQuery(Name = "page")] int page = 1,
                                                [FromQuery(Name = "page-size")] int pageSize = 10
                                                )
        {
            List<RangeFilter<int?>> parsedAgeRanges = new List<RangeFilter<int?>>();
            if (agesRanges != null && agesRanges.Length > 0)
            {
                foreach (var strAge in agesRanges)
                {
                    var ageRange = ParseIntRangeFromQueryString(strAge);
                    if(ageRange.Item1.HasValue || (ageRange.Item2.HasValue))
                    {
                        parsedAgeRanges.Add(new RangeFilter<int?>() { LowerBound = ageRange.Item1, UpperBound = ageRange.Item2 });
                    }
                }
            }

            List<RangeFilter<int?>> parsedSalaryRanges = new List<RangeFilter<int?>>();
            if (salaryRanges != null && salaryRanges.Length > 0)
            {
                foreach (var strSalary in salaryRanges)
                {                   
                    var salaryRange = ParseIntRangeFromQueryString(strSalary);
                    if (salaryRange.Item1.HasValue || (salaryRange.Item2.HasValue))
                    {
                        parsedSalaryRanges.Add(new RangeFilter<int?>() { LowerBound = salaryRange.Item1, UpperBound = salaryRange.Item2 });
                    }
                }
            }

            List<RangeFilter<string>> parsedDateRanges = new List<RangeFilter<string>>();
            if (strJoins != null && strJoins.Length > 0)
            {
                
                foreach (var strJoin in strJoins)
                {
                    var dateRante = ParseDateMathRange(strJoin);
                    if (!string.IsNullOrEmpty(dateRante.Item1) || !string.IsNullOrEmpty(dateRante.Item2))
                    {
                        parsedDateRanges.Add(new RangeFilter<string>() { LowerBound = dateRante.Item1, UpperBound = dateRante.Item2 });
                    }
                }
            }

            EmployeesSearchRequest searchRequest = new EmployeesSearchRequest()
            {
                Page = page,
                PageSize = pageSize,
                Filters = new SearchFilters()
                {
                    Designations = designations?.ToArray(),
                    Gender = genders?.ToArray(),
                    MaritalStatus = maritialStatuses?.ToArray(),
                    AgeRanges = parsedAgeRanges,
                    SalaryRanges = parsedSalaryRanges,
                    DateRanges = parsedDateRanges,
                    QueryString = query,
                }
            };
            var searchResponse = await _searchService.SearchAsync(searchRequest, cancellationToken);
            SearchFiltersViewModel filtersViewModel = BuildFiltersViewModel(searchResponse.Filters);

            string currentUrl = Request.GetDisplayUrl();
            List<(string, string)> currentFilters = UrlHelper.GetActiveFilters(currentUrl);
            var QueryFilter = new QueryFilterViewModel()
            {
                Query = UrlHelper.GetFilterValue(currentFilters, "q"),
                ActiveFilters = UrlHelper.RemoveFilter(currentFilters, "q"),
            };

            string prevPageUrl = string.Empty;
            string nextPageUrl = string.Empty;

            if (searchResponse.PageInfo.CurrentPage > 1)
            {
                prevPageUrl = UrlHelper.SetFilter(currentFilters, "page", (searchResponse.PageInfo.CurrentPage - 1).ToString()).ToUrl(currentUrl);
            }

            if (searchResponse.PageInfo.CurrentPage < searchResponse.PageInfo.TotalPages)
            {
                nextPageUrl = UrlHelper.SetFilter(currentFilters, "page", (searchResponse.PageInfo.CurrentPage + 1).ToString()).ToUrl(currentUrl);
            }


            var activeFilters = filtersViewModel.Designations
                                  .Concat(filtersViewModel.Genders)
                                  .Concat(filtersViewModel.MaritialStatusTypes)
                                  .Concat(filtersViewModel.SalaryRanges)
                                  .Concat(filtersViewModel.AgeRanges)
                                  .Concat(filtersViewModel.DateOfJoiningRanges)
                                  .Where(x => x.IsActive)
                                  .ToList();
            SearchViewModel vm = new SearchViewModel()
            {
                Results = searchResponse.Results,
                PageInfo = new PagingInfoViewModel()
                {
                    CurrentPage = searchResponse.PageInfo.CurrentPage,
                    EndElement = searchResponse.PageInfo.EndElement,
                    PageSize = searchResponse.PageInfo.PageSize,
                    StartElement = searchResponse.PageInfo.StartElement,
                    TotalCount = searchResponse.PageInfo.TotalCount,
                    TotalPages = searchResponse.PageInfo.TotalPages,
                    PrevPageUrl = prevPageUrl,
                    NextPageUrl = nextPageUrl,
                },
                Filters = filtersViewModel,
                SearchBoxQueryViewModel = QueryFilter,
                ActiveFilters = activeFilters,
            };
            return View(vm);
        }

        [HttpGet]
        [Route("api/search/autocomplete")]
        public async Task<IActionResult> AutoComplete([FromQuery(Name = "q")] string query, CancellationToken cancellationToken)
        {
            var appRequest = new EmployeesSearchAsYouTypeRequest() { Query = query };
            var response = await _searchService.SearchAsYouType(appRequest, cancellationToken);
            return Json(response);
        }

        private SearchFiltersViewModel BuildFiltersViewModel(AggregationsResponse response)
        {
            string currentUrl = Request.GetDisplayUrl();
            List<(string, string)> currentFilters = UrlHelper.GetActiveFilters(currentUrl);
            currentFilters = UrlHelper.RemoveFilter(currentFilters, "page");

            List<AggregationBaseViewModel> designationsViewModels = new List<AggregationBaseViewModel>();
            List<AggregationBaseViewModel> gendersViewModels = new List<AggregationBaseViewModel>();
            List<AggregationBaseViewModel> maritialStatusTypesViewModels = new List<AggregationBaseViewModel>();
            List<AggregationBaseViewModel> salaryRangesViewModels = new List<AggregationBaseViewModel>();
            List<AggregationBaseViewModel> ageRangesViewModels = new List<AggregationBaseViewModel>();
            List<AggregationBaseViewModel> dateOfJoiningRangesViewModels = new List<AggregationBaseViewModel>();

            if (response != null)
            {
                if (response.Designations != null)
                {
                    designationsViewModels.AddRange(
                        response.Designations
                            .Select(x => new AggregationBaseViewModel()
                            {
                                Name = x.Name,
                                Count = x.Count,
                                GoUrl = UrlHelper.ToogleFilter(currentFilters, ("designation", x.Name)).ToUrl(currentUrl),
                                IsActive = UrlHelper.IsActiveFilter(currentFilters, ("designation", x.Name)),
                                DisplayName = "Designation",
                            })
                        );
                }

                if (response.Genders != null)
                {
                    gendersViewModels.AddRange(
                        response.Genders
                            .Select(x => new AggregationBaseViewModel()
                            {
                                Name = x.Name,
                                Count = x.Count,
                                GoUrl = UrlHelper.ToogleFilter(currentFilters, ("gender", x.Name)).ToUrl(currentUrl),
                                IsActive = UrlHelper.IsActiveFilter(currentFilters, ("gender", x.Name)),
                                DisplayName = "Gender",
                            })
                        );
                }

                if (response.MaritialStatusTypes != null)
                {
                    maritialStatusTypesViewModels.AddRange(
                        response.MaritialStatusTypes
                            .Select(x => new AggregationBaseViewModel()
                            {
                                Name = x.Name,
                                Count = x.Count,
                                GoUrl = UrlHelper.ToogleFilter(currentFilters, ("maritial-status", x.Name)).ToUrl(currentUrl),
                                IsActive = UrlHelper.IsActiveFilter(currentFilters, ("maritial-status", x.Name)),
                                DisplayName = "Maritial Status",
                            })
                        );
                }

                if (response.SalaryRanges != null)
                {
                    salaryRangesViewModels.AddRange(
                        response.SalaryRanges
                            .Select(x => new AggregationBaseViewModel()
                            {
                                Name = x.Name,
                                Count = x.Count,
                                GoUrl = UrlHelper.ToogleRangeFilter(currentFilters, "salary", x.LowerValue?.ToString(), x.UpperValue?.ToString())
                                        .ToUrl(currentUrl),
                                IsActive = UrlHelper.IsActiveRangeFilter(currentFilters, "salary", x.LowerValue?.ToString(), x.UpperValue?.ToString()),
                                DisplayName = "Salary",
                            })
                        );
                }

                if (response.AgeRanges != null)
                {
                    ageRangesViewModels.AddRange(
                        response.AgeRanges
                            .Select(x => new AggregationBaseViewModel()
                            {
                                Name = x.Name,
                                Count = x.Count,
                                GoUrl = UrlHelper.ToogleRangeFilter(currentFilters, "age", x.LowerValue?.ToString(), x.UpperValue?.ToString())
                                        .ToUrl(currentUrl),
                                IsActive = UrlHelper.IsActiveRangeFilter(currentFilters, "age", x.LowerValue?.ToString(), x.UpperValue?.ToString()),
                                DisplayName = "Age",
                            })
                        );
                }

                if (response.DateOfJoiningRanges != null)
                {
                    dateOfJoiningRangesViewModels.AddRange(
                        response.DateOfJoiningRanges
                            .Select(x => new AggregationBaseViewModel()
                            {
                                Name = x.Name,
                                Count = x.Count,
                                GoUrl = UrlHelper.ToogleRangeFilter(currentFilters, "joining", x.LowerValue, x.UpperValue)
                                        .ToUrl(currentUrl),
                                IsActive = UrlHelper.IsActiveRangeFilter(currentFilters, "joining", x.LowerValue, x.UpperValue),
                                DisplayName = "Joining",
                            })
                        );
                }

            }



            var viewModel = new SearchFiltersViewModel()
            {
                Designations = designationsViewModels,
                Genders = gendersViewModels,
                MaritialStatusTypes = maritialStatusTypesViewModels,
                SalaryRanges = salaryRangesViewModels,
                AgeRanges = ageRangesViewModels,
                DateOfJoiningRanges = dateOfJoiningRangesViewModels,
            };

            return viewModel;
        }
    }
}