using System;
using System.Linq;
using System.Collections.Generic;
using uni_elastic_manager.infra;
using uni_elastic_manager.Infra;

namespace uni_elastic_manager
{
    class Program
    {
        static void Main(string[] args)
        {
            var collector = new Prometheus(new Settings());
            var metrics = collector.Collect();
            PrintMetrics(metrics);

            var analyzer = new ArimaWithRApi(2, 2, 2);
            analyzer.Calculate(metrics.Select(x => x.Value).ToArray());
        }

        static void PrintMetrics(List<Model.CpuMetric> metrics)
        {
            foreach (var metric in metrics)
            {
                Console.WriteLine($"{metric.Time} - {metric.DateTime} - {metric.Value}");
            }
        }
    }
}
