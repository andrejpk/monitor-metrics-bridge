using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace monitor_metrics_bridge
{
  public class Program
  {

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

      // Read Azure connection args, set up MetricsReader
      Console.WriteLine("Reading config from env variables");
      var tenantID = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
      var clientID = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
      var secret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
      var subscriptionID = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");

      MetricsReader metricsReader = new MetricsReader(tenantID, clientID, secret, subscriptionID);

      // Read sender args, set up sender
      var targetEndpoint = Environment.GetEnvironmentVariable("TARGET_ENDPOINT");
      PowerBIMetricsSender sender = new PowerBIMetricsSender();

      int intervalSeconds = 60;

      var metricsOut = await metricsReader.ReadMetrics(resourceID, intervalSeconds);

      Console.WriteLine($"Sending data to PowerBI endpoint {targetEndpoint}");
      await sender.SendMetrics(targetEndpoint, metricsOut);
      Console.WriteLine($"Success");

    }
  }
}
