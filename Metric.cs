using System;
using System.Text.Json.Serialization;

namespace monitor_metrics_bridge
{
  public class Metric
  {
    public Metric(string resourceGroupName, string resourceName, string metricName, DateTime timeStamp, Double? data) =>
      (this.ResourceGroupName, this.ResourceName, this.MetricName, this.TimeStamp, this.Data) =
      (resourceGroupName, resourceName, metricName, timeStamp, data);

    public string ResourceGroupName { get; }

    public string ResourceName { get; }
    public string MetricName { get; }
    public DateTime TimeStamp { get; }
    public Double? Data { get; }
  }

}