using System;
using uni_arima.infra;

namespace uni_arima.Model
{
    public class CpuMetric
    {
        public long Time { get; set; }
        public DateTime DateTime { get { return Prometheus.FromUnixTime(Time); } }
        public string Value { get; set; }
    }
}