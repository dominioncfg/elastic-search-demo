using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticSearchDemo.Web.Infra.Helpers
{
    public static class UrlHelper
    {
        public static string ToUrl(this List<(string, string)> filters, string currentUrl)
        {
            Uri currentUri = new Uri(currentUrl);
            string newUrl = currentUri.GetLeftPart(UriPartial.Path);
            if (filters.Count > 0)
            {
                foreach (var item in filters)
                {
                    newUrl = QueryHelpers.AddQueryString(newUrl, item.Item1, item.Item2);
                }
            }
            return newUrl;
        }
        public static List<(string, string)> GetActiveFilters(string currentUrl)
        {
            Uri currentUri = new Uri(currentUrl);

            List<(string, string)> currentParams = new List<(string, string)>();
            if (!string.IsNullOrEmpty(currentUri.Query))
            {
                var something = QueryHelpers.ParseQuery(currentUri.Query);
                foreach (var keyValue in something)
                {
                    string key = keyValue.Key;
                    string[] values = keyValue.Value.ToArray();
                    foreach (var val in values)
                    {
                        currentParams.Add((key, val));
                    }
                }
            }
            return currentParams;
        }
        public static List<(string, string)> ToogleFilter(List<(string, string)> currentParams, (string, string) currentFilter)
        {

            //Add Toogle Filters
            List<(string, string)> response = new List<(string, string)>(currentParams);
            if (currentFilter != default)
            {
                var newFilters = currentParams
                                      .Where(filter => filter.Item1 != currentFilter.Item1 || filter.Item2 != currentFilter.Item2)
                                      .ToList();

                if (currentParams.Count == newFilters.Count)
                {
                    newFilters.Add(currentFilter);
                }
                response = newFilters;
            }
            return response;
        }
        public static List<(string, string)> ToogleRangeFilter(List<(string, string)> currentParams, string filterName, string lowerBound, string upperBound)
        {
            string filterValue = GetRangeFilterValue(lowerBound, upperBound);
            return ToogleFilter(currentParams, (filterName, filterValue));
        }
        private static string GetRangeFilterValue(string lowerBound, string upperBound)
        {
            return $"{(lowerBound ?? "x")}-{(upperBound ?? "x")}";
        }
        public static List<(string, string)> RemoveFilter(List<(string, string)> currentParams, string currentFilter)
        {
            //Remove the filter
            if (currentFilter != default)
            {
                currentParams = currentParams
                                     .Where(filter => filter.Item1 != currentFilter)
                                     .ToList();


            }
            return currentParams;
        }
        public static List<(string, string)> SetFilter(List<(string, string)> currentFilters, string filterName, string filterValue)
        {
            if (currentFilters != default)
            {
                List<(string, string)> response = new List<(string, string)>(currentFilters);
                bool found = false;
                for (int i = 0; i < response.Count; i++)
                {
                    var filter = response[i];
                    if (filter.Item1 == filterName)
                    {
                        response[i] = (filterName, filterValue);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    response.Add((filterName, filterValue));
                }
                return response;
            }
            return currentFilters;
        }
        public static bool IsActiveFilter(List<(string, string)> currentFilters, (string, string) filter)
        {
            var exists = currentFilters
                                  .Exists(f => f.Item1 == filter.Item1 && f.Item2 == filter.Item2);
            return exists;
        }
        public static bool IsActiveRangeFilter(List<(string, string)> currentFilters, string filterName, string lowerBound, string upperBound)
        {
            string filterValue = GetRangeFilterValue(lowerBound, upperBound);
            var exists = currentFilters
                                  .Exists(f => f.Item1 == filterName && f.Item2 == filterValue);
            return exists;
        }
        public static string GetFilterValue(List<(string, string)> filters, string filterName)
        {
            return filters.FirstOrDefault(x => x.Item1 == filterName).Item2;
        }
    }
}
