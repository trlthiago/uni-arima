using System;
using uni_elastic_manager.infra;

namespace uni_elastic_manager
{
    class Program
    {
        static void Main(string[] args)
        {
            var collector = new Prometheus(new Settings());
            var metrics = collector.Collect();
            foreach (var metric in metrics)
            {
                Console.WriteLine($"{metric.Time} - {metric.DateTime} - {metric.Value}");
            }
        }
    }
}
