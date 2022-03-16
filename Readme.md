# Elastic Search Demo

This is just and .Net Core + Elastic Search Demo.
It contains a seed process that imports 100k registries to a sql  and elasticsearch dbs and web interface to search.

## Configuration

User Secrets Seed Proccess:
User Secrets Web:

```json
{
  "ElasticSearch": {
    "ServerEndpoint": "http://localhost:9200",
    "EmployeesIndexName": "employees"
  },
  "Databases": {
    "EmployeesConnectionString": "Data Source=.;Initial Catalog=Employees;Integrated Security=True;Connect Timeout=30;"
  }
}
```
