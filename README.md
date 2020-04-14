# Azure Monitor Metrics Extract Sample

This sample demonstrates extracting Azure Monitor metrics data using the Azure RM Fluent C# SDK with .NET Core and
sending it to a PowerBI data endpoint for live dashboards.

## Usage

Create a .env file and/or set the following environemnt variables:

```
AZURE_TENANT_ID={guid}
AZURE_CLIENT_ID={guid}
AZURE_CLIENT_SECRET={guid}
AZURE_SUBSCRIPTION_ID={guid}
TARGET_ENDPOINT={http post target URI}
```

Run the component with a single CLI argument containg the Resource ID on the metrics (format looks something like
`/subscriptions/0000000-0000-0000-0000-000000000/resourceGroups/rgname/providers/Microsoft.Service/xxx`) 

``` bash
dotnet run --- /subscriptions/0000000-0000-0000-0000-000000000/resourceGroups/rgname/providers/Microsoft.Service/xxx
```