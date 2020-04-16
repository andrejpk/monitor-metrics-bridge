﻿using System;
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
  public class Program
  {
    private static readonly HttpClient client = new HttpClient();

    public static async Task Main(string[] args)
    {
      DotNetEnv.Env.Load();

      // supported metrics list: https://docs.microsoft.com/en-us/azure/azure-monitor/platform/metrics-supported

      // Read CLI args
      if (args.Length < 1)
      {
        throw new ArgumentException($"Usage: AzureMonitorCSharpExamples <resourceId> <namespace> (num supplied: {args.Length})");
      }
      string resourceID = args[0];

      // Create Azure ARM connection, auth
      Console.WriteLine("Reading config from env variables");
      var tenantID = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
      var clientID = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
      var secret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
      var subscriptionID = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");

      MetricsReader metricsReader = new MetricsReader(tenantID, clientID, secret, subscriptionID);

      // Read other args
      var targetEndpoint = Environment.GetEnvironmentVariable("TARGET_ENDPOINT");

      int intervalSeconds = 60;

      var metricsOut = await metricsReader.ReadMetrics(resourceID, intervalSeconds);

      Console.WriteLine($"Sending data to PowerBI endpoint {targetEndpoint}");
      await SendMetrics(targetEndpoint, metricsOut);
      Console.WriteLine($"Success");

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
