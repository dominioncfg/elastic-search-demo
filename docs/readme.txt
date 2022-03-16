1- Migration Commands (Infraestructura Folder):
dotnet ef --startup-project ../ElasticSearchDemo.Web/ migrations add Initial
dotnet ef --startup-project ../ElasticSearchDemo.Web/ database update


User Secrets Seed Proccess:
User Secrets Web
{
  "ElasticSearch": {
    "ServerEndpoint": "http://localhost:9200",
    "EmployeesIndexName": "employees"
  },
  "Databases": {
    "EmployeesConnectionString": "Data Source=.;Initial Catalog=Employees;Integrated Security=True;Connect Timeout=30;"
  }
}