namespace uni_elastic_manager.infra
{
    public class PrometheusWindowsCollector : PrometheusCollector
    {
        public PrometheusWindowsCollector(Settings settings) : base(settings)
        {
        }

        protected override string GetCpuMetric(long start, long end)
        {
            return Get($"api/v1/query_range?query=100%20-%20avg%20(irate({_settings.Metric}%7Binstance%3D%22{_settings.Instance}%22%2Cmode%3D%22idle%22%7D%5B60s%5D))%20*%20100&start={start}&end={end}&step=15");
        }
    }
}