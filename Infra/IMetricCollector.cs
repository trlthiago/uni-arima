using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using uni_elastic_manager.Model;

namespace uni_elastic_manager.infra
{
    public interface IMetricCollector
    {
        List<CpuMetric> Collect();
    }

    public class Prometheus : IMetricCollector
    {
        private readonly Settings _settings;
        public Prometheus(Settings settings)
        {
            _settings = settings;
        }

        private string Get(string query)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri($"http://{_settings.Prometheus}");
            var response = client.GetAsync(query).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        private string GetCpuMetric(long start, long end)
        {
            return Get($"api/v1/query_range?query=100%20-%20avg%20(irate({_settings.Metric}%7Binstance%3D%22{_settings.Instance}%22%2Cmode%3D%22idle%22%7D%5B60s%5D))%20*%20100&start={start}&end={end}&step=15");
        }

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
            var parsedObject = JObject.Parse(response);
            var values = parsedObject["data"]["result"][0]["values"];
            var metrics = new List<CpuMetric>();
            foreach (JArray item in values)
            {
                var metric = new CpuMetric
                {
                    Time = long.Parse(item[0].ToString()),
                    Value = item[1].ToString()
                };
                metrics.Add(metric)
            ;
            }
            return metrics;
        }

        public List<CpuMetric> Collect()
        {
            var start = ToUnixTime(DateTime.UtcNow.AddMinutes(-15));
            var end = ToUnixTime(DateTime.UtcNow);
            var response = GetCpuMetric(start, end);
            return ParseCpuMetrics(response);
        }
    }
}