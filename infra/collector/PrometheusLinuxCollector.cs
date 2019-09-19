namespace uni_elastic_manager.infra
{
    public class PrometheusLinuxCollector : PrometheusCollector
    {
        public PrometheusLinuxCollector(Settings settings) : base(settings)
        {

        }

        protected override string GetCpuMetric(long start, long end)
        {
            return Get($"api/v1/query_range?query=avg%20(irate({_settings.Metric}%7Binstance%3D%22{_settings.Instance}%22%2C%20name%3D~%22{_settings.Name}%22%7D%5B30s%5D))%20*%20100&start={start}&end={end}&step=15");
        }
    }
}