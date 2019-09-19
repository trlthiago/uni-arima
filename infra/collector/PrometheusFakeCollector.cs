using System;
using System.Collections.Generic;
using uni_elastic_manager.Model;

namespace uni_elastic_manager.infra
{

    public class PrometheusFakeCollector : PrometheusCollector
    {
        public PrometheusFakeCollector(Settings settings) : base(settings)
        {
        }

        protected override string GetCpuMetric(long start, long end)
        {
            return "100.0";
        }

        public override List<CpuMetric> Collect()
        {
            return new List<CpuMetric>
            {
                new CpuMetric{Time = DateTime.UtcNow.Ticks,Value="100"},
                new CpuMetric{Time = DateTime.UtcNow.Ticks,Value="100"},
                new CpuMetric{Time = DateTime.UtcNow.Ticks,Value="100"},
                new CpuMetric{Time = DateTime.UtcNow.Ticks,Value="100"},
                new CpuMetric{Time = DateTime.UtcNow.Ticks,Value="100"},
            };
        }
    }
}