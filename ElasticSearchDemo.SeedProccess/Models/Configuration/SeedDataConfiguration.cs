namespace ElasticSearchDemo.SeedProccess.Models.Configuration
{
    public class SeedDataConfiguration
    {
        public readonly static string SectionName = "SeedData";
        public string ImportFileName { get; set; }

        public string OutPutFileName { get; set; }
    }
}