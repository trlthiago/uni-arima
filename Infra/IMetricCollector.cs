using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using uni_arima.Model;

namespace uni_arima.infra
{
    public interface IMetricCollector
    {
        List<CpuMetric> Collect();
    }

    public class Prometheus : IMetricCollector
    {
        private string _prometheusServer;
        public Prometheus(string prometheusServer)
        {
            _prometheusServer = prometheusServer;

        }

        private string Get(string instance, string query, long start, long end)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri($"http://{_prometheusServer}:9090");
            var response = client.GetAsync($"api/v1/query_range?query=100%20-%20avg%20(irate(wmi_cpu_time_total%7Binstance%3D%22{instance}%22%2Cmode%3D%22idle%22%7D%5B60s%5D))%20*%20100&start={start}&end={end}&step=15").Result;
            return response.Content.ReadAsStringAsync().Result;
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
            var response = Get("ubapp01", "", start, end);
            return ParseCpuMetrics(response); ;
        }
    }
}