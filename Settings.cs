using System;

namespace uni_elastic_manager
{
    public class Settings
    {
        public string Prometheus => Environment.GetEnvironmentVariable("Prometheus");
        public string Instance => Environment.GetEnvironmentVariable("Instance");
        public string Metric => Environment.GetEnvironmentVariable("Metric");
    }
}