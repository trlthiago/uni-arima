using System.Collections.Generic;
using uni_elastic_manager.Model;

namespace uni_elastic_manager.infra
{
    public interface IMetricCollector
    {
        List<CpuMetric> Collect();
    }
}