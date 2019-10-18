using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using uni_elastic_manager.Model;

namespace uni_elastic_manager.infra
{
    public abstract class PrometheusCollector : IMetricCollector
    {
        protected readonly Settings _settings;
        protected readonly long _start;
        public PrometheusCollector(Settings settings)
        {
            _start = ToUnixTime(DateTime.UtcNow);
            _settings = settings;
        }

        protected string Get(string query)
        {
            Console.WriteLine(query);
            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri($"http://{_settings.Prometheus}");
            var response = client.GetAsync(query).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        protected abstract string GetCpuMetric(long start, long end);

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public long ToUnixTime(DateTime datetime)
        {
            return ((DateTimeOffset)datetime).ToUnixTimeSeconds();
        }

        public List<CpuMetric> ParseCpuMetrics(string response)
        {
            var metrics = new List<CpuMetric>();
            var parsedObject = JObject.Parse(response);
            if (!parsedObject["data"]["result"].HasValues)
                return metrics;

            var values = parsedObject["data"]["result"][0]["values"];
            foreach (JArray item in values)
            {
                var value = item[1].ToString();
                if (value == "NaN")
                    continue;
                if (_settings.OS == "linux")
                {
                    value = value.Replace(".", ",");
                }
                var metric = new CpuMetric
                {
                    Time = long.Parse(item[0].ToString()),
                    Value = value
                };
                metrics.Add(metric);
            }
            return metrics;
        }

        public virtual List<CpuMetric> Collect()
        {
            var response = GetCpuMetric(_start, ToUnixTime(DateTime.UtcNow));
            return ParseCpuMetrics(response);
        }
    }
}