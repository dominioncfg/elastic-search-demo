namespace ElasticSearchDemo.Web.Models.Configuration
{
    public class ElasticSearchConfiguration
    {
        public readonly static string SectionName = "ElasticSearch";
        public string ServerEndpoint { get; set; }
        public string EmployeesIndexName { get; set; }
    }
}
