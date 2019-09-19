using uni_elastic_manager;
using uni_elastic_manager.infra;

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
    }
}