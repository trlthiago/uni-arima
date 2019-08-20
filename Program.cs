using System;
using uni_arima.infra;

namespace uni_arima
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
