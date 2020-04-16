using System;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Azure Management dependencies
using Microsoft.Rest.Azure.Authentication;
using Microsoft.Rest.Azure.OData;

// These examples correspond to the Monitor .Net SDK versions 0.16.0-preview and 0.16.1-preview
// Those versions include the single-dimensional metrics API.
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.Monitor;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Monitor.Fluent;
using Microsoft.Azure.Management.Monitor.Fluent.Models;

namespace monitor_metrics_bridge
{
  public class MetricsReader
  {
    private static readonly HttpClient client = new HttpClient();
    private string subscriptionID;
    // private string TenantID, ClientID, Secret, SubscriptionID;
    private AzureCredentials _credentials;
    private IAzure _azure;

    public MetricsReader(string tenantID, string clientID, string secret, string subscriptionID)
    {
      this.subscriptionID = subscriptionID;
      // (TenantID, ClientID, Secret, SubscriptionID) = (tenantID, clientID, secret, subscriptionID);
      Console.WriteLine($"Authenticating to Azure");
      _credentials = SdkContext.AzureCredentialsFactory
         .FromServicePrincipal(clientID, secret, tenantID, AzureEnvironment.AzureGlobalCloud);
      _azure = Azure
        .Authenticate(_credentials)
        .WithDefaultSubscription();
    }

    public async Task<IList<Metric>> ReadMetrics(string resourceID, int windowSeconds, DateTime? endTime = null)
    {
      // supported metrics list: https://docs.microsoft.com/en-us/azure/azure-monitor/platform/metrics-supported

      // Get a list of available metrics, types
      Console.WriteLine($"Reading metrics for resource ${resourceID}");

      var metricDefs = await _azure.MetricDefinitions.ListByResourceAsync(resourceID);

      var endTimeChecked = (endTime ?? DateTime.UtcNow);
      var startTime = endTimeChecked.AddSeconds(-windowSeconds);
      var metricsOut = new List<Metric>();
      Console.WriteLine($"Reading metrics from {startTime} to {endTimeChecked}");

      foreach (var md in metricDefs)
      {
        Console.WriteLine($"{md.Name.Value} : {md.Unit} : {md.PrimaryAggregationType.ToString()}");
        var metricsColl = await md.DefineQuery()
          .StartingFrom(startTime)
          .EndsBefore(endTimeChecked)
          .ExecuteAsync();

        foreach (var metric in metricsColl.Metrics)
        {
          foreach (var ts in metric.Timeseries)
          {
            foreach (var data in ts.Data)
            {
              Console.WriteLine($"Total: {data.Total} Average: {data.Average}Count: {data.Count} Timestamp: {data.TimeStamp.ToShortTimeString()}");
              metricsOut.Add(new Metric
              {
                SubscriptionID = subscriptionID,
                ResourceGroupName = "",
                ResourceName = resourceID,
                MetricName = md.Name.Value,
                WindowStart = startTime,
                WindowEnd = endTimeChecked,
                TimeStamp = data.TimeStamp,
                Total = data.Total,
                Average = data.Average,
                Count = data.Count,
                Maximum = data.Maximum,
                Minimum = data.Minimum
              });
            }
          }
        }
      }

      return metricsOut;
    }

    static async Task SendMetrics(string endpoint, IEnumerable<Metric> metrics)
    {
      var outJson = JsonSerializer.Serialize(metrics);
      Console.WriteLine($"Data:\n{outJson}");
      var result = await client.PostAsync(endpoint, new StringContent(outJson, Encoding.UTF8, "application/json"));
      var resultBodyString = await result.Content.ReadAsStringAsync();
      if (!result.IsSuccessStatusCode)
      {
        throw new Exception($"Error sending data: {result.ReasonPhrase}; status ${result.StatusCode}; \nbody: ${resultBodyString}");
      }
    }

  }
}
