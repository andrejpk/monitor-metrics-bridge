using System;
using System.Text.Json.Serialization;

namespace monitor_metrics_bridge
{
  public class Metric
  {
    public string SubscriptionID { get; set; }
    public string ResourceGroupName { get; set; }
    public string ResourceName { get; set; }
    public string MetricName { get; set; }
    public DateTime WindowStart { get; set; }
    public DateTime WindowEnd { get; set; }
    public DateTime TimeStamp { get; set; }
    public Double? Total { get; set; }
    public Double? DeltaTotal { get; set; }
    public Double? Count { get; set; }
    public Double? Minimum { get; set; }
    public Double? Maximum { get; set; }
    public Double? Average { get; set; }
  }

}