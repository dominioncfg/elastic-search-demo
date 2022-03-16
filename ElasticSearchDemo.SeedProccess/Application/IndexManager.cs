using ElasticSearchDemo.DomainModel.Models;
using ElasticSearchDemo.DomainModel.Repositories;
using ElasticSearchDemo.DomainModel.SeedWork.Extensions;
using ElasticSearchDemo.SeedProccess.Models.Configuration;
using ElasticSearchDemo.SeedProccess.Repositories;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElasticSearchDemo.SeedProccess.Application
{
    public class IndexManager
    {
        private readonly IElasticClient _elasticClient;
        private readonly IEmployeesRepository _employeesTargetRepository;
        private readonly EmployeesFileRepository _employeesSourceRepository;
        private readonly ElasticSearchConfiguration _settings;
        private readonly SeedDataConfiguration _seedSettings;

        public IndexManager(
                                IElasticClient elasticClient,
                                IEmployeesRepository employeesTargetRepository,
                                EmployeesFileRepository employeesSourceRepository,
                                IOptions<ElasticSearchConfiguration> settings,
                                IOptions<SeedDataConfiguration> seedSettings
                            )
        {
            _elasticClient = elasticClient;
            _employeesTargetRepository = employeesTargetRepository;
            _employeesSourceRepository = employeesSourceRepository;
            _settings = settings.Value;
            _seedSettings = seedSettings.Value;
        }

        #region Elastic Search      
        public async Task CreateIndexIfNotExistAsync()
        {
            var response = await _elasticClient.Indices.ExistsAsync(_settings.EmployeesIndexName);

            if (!response.Exists)
            {
                await _elasticClient.Indices
                      .CreateAsync(_settings.EmployeesIndexName, index =>
                          index
                            .Settings(se => se
                                        .NumberOfReplicas(_settings.NumberOfReplicas)
                                        .NumberOfShards(_settings.NumberOfShards)
                                        .Analysis
                                        (a=>a
                                            .TokenFilters(f=>f
                                                 .Stemmer("lang_stemmer",
                                                            selector=> selector
                                                            .Language("english")
                                                 )
                                                 .Stemmer("lang_possessive_stemmer",
                                                            selector => selector
                                                            .Language("possessive_english")
                                                 )
                                            )
                                            .Analyzers(analyzers=> analyzers
                                                .Custom("custom_stemmer",selector=> selector
                                                            .Tokenizer("standard")
                                                            .Filters(
                                                                        "lang_possessive_stemmer",
                                                                        "lowercase",
                                                                        "lang_stemmer"
                                                                    )
                                                        )
                                            )
                                        )
                                     )
                            .Map<Employee>(m => m
                                                .AutoMap<Employee>()
                                                .Properties(p=>p
                                                                .Text(t=>t
                                                                        .Name(nameof(Employee.Interests).ToLower())
                                                                        .Analyzer("custom_stemmer")
                                                                        .Fields(subfields=>
                                                                            subfields.SearchAsYouType(selector=>selector
                                                                                .Name("autocomplete")
                                                                                .MaxShingleSize(4)
                                                                            )
                                                                        )
                                                                     )
                                                           )

                                                .Dynamic(DynamicMapping.Strict)
                                          )
                        );
            }

        }
        public void IndexDocs(List<Employee> employees)
        {
            var bulkAllObservable =
                _elasticClient.BulkAll(employees, b =>
                    b
                    .Index(_settings.EmployeesIndexName)
                    .BackOffTime("30s")
                    .BackOffRetries(2)
                    .RefreshOnCompleted()
                    .MaxDegreeOfParallelism(Environment.ProcessorCount)
                    .Size(1000)
                )
            .Wait(TimeSpan.FromMinutes(15), next => { });
        }
        public async Task IndexAsync(List<Employee> employees)
        {
            await CreateIndexIfNotExistAsync();
            IndexDocs(employees);
        }
        #endregion

        #region SqlServer
        public async Task UpdateDatabaseAsync(List<Employee> source)
        {
            HashSet<int> ids = new HashSet<int>((await _employeesTargetRepository.GetAllAsync(default)).Select(x => x.Id));

            var updateEntites = source.Where(x => !ids.Contains(x.Id)).ToList();
            foreach (var batch in updateEntites.Batch(1000))
            {
                await _employeesTargetRepository.AddBatchAsync(batch, default);
            }
        }
        #endregion

        #region Sanitize the Input
        private List<Employee> SanitizeData(List<Employee> employees)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            Random random = new Random();


            foreach (var e in employees)
            {
                e.FirstName = currentCulture.TextInfo.ToTitleCase(e.FirstName.ToLower());
                e.LastName = currentCulture.TextInfo.ToTitleCase(e.LastName.ToLower());
                RandomChangeDate(random, e);
            }
            return employees;
        }

        private static void RandomChangeDate(Random random, Employee e)
        {
            DateTime today = DateTime.Today;
            List<DateTime> origins = new List<DateTime>()
            {
                today,
                today.AddMonths(-6),
                today.AddYears(-1),
                today.AddYears(-3)
            };


            var changeDateRandonNumber = random.Next(0, 100);
            var changeDate = changeDateRandonNumber >= 70 && changeDateRandonNumber < 80;
            if (changeDate)
            {
                int originInteger = random.Next(0, origins.Count);
                var dateOrigin = origins[originInteger];

                int maxDays = (dateOrigin - dateOrigin.AddMonths(-6)).Days;
                int offset = random.Next(0, maxDays);
                var resDate = dateOrigin.AddDays(offset * -1);
                e.DateOfJoining = resDate;
            }
        }
        #endregion

        #region Save Output
        private async Task SaveToOutputAsync(List<Employee> employees, string fileInfo)
        {
            string jsonContent = JsonSerializer.Serialize(employees);
            await File.WriteAllTextAsync(fileInfo, jsonContent);
        }
        #endregion


        internal async Task RunAsync()
        {
            string saveFileName = Path.Combine(Environment.CurrentDirectory, _seedSettings.OutPutFileName);

            var employees = await _employeesSourceRepository.GetAllAsync(default);
            //employees = SanitizeData(employees);
            await IndexAsync(employees);
            await UpdateDatabaseAsync(employees);
            await SaveToOutputAsync(employees, saveFileName);
        }
    }
}
