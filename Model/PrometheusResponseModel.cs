using System;
using uni_elastic_manager.infra;

namespace uni_elastic_manager.Model
{
    public class CpuMetric
    {
        public long Time { get; set; }
        public DateTime DateTime { get { return PrometheusCollector.FromUnixTime(Time); } }
        public string Value { get; set; }
    }
}