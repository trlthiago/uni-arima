namespace uni_elastic_manager.infra
{
    public static class PrometheusFactory
    {
        public static PrometheusCollector GetInstance(Settings settings)
        {
            if (settings.OS == "windows") return new PrometheusWindowsCollector(settings);
            return new PrometheusLinuxCollector(settings);
        }
    }
}