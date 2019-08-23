using System.Linq;
using uni_arima.infra.runnable;
using uni_elastic_manager.infra;
using uni_elastic_manager.Infra;

namespace uni_elastic_manager
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings();
            var collector = PrometheusFactory.GetInstance(settings);
            var evaluator = RunnableFactory.GetInstance(settings);
            var analyzer = new ArimaWithRBinary(2, 2, 2);
            while (true)
            {
                var metrics = collector.Collect();
                evaluator.Evaluate(analyzer.Calculate(metrics.Select(x => x.Value).ToArray()));
                System.Threading.Thread.Sleep(15000);
            }

        }

    }
}
