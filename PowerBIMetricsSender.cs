using System;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace monitor_metrics_bridge
{
  public class PowerBIMetricsSender
  {
    private static readonly HttpClient client = new HttpClient();

    public async Task SendMetrics(string endpoint, IEnumerable<Metric> metrics)
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
