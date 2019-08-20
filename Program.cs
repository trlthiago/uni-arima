using System;

namespace uni_arima
{
    class Program
    {
        static void Main(string[] args)
        {
            var collector = new infra.Prometheus("localhost");
            var metrics = collector.Collect();
            foreach (var metric in metrics)
            {
                Console.WriteLine($"{metric.Time} - {metric.DateTime} - {metric.Value}");
            }
        }
    }
}
