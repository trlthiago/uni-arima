using log4net;
using log4net.Config;
using System.Linq;
using System.Reflection;
using uni_arima.infra.evaluator;
using uni_elastic_manager.infra;
using uni_elastic_manager.infra.runnable;

namespace uni_elastic_manager
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = ConfigureLog();
            var settings = new Settings();
            var collector = PrometheusFactory.GetInstance(settings);
            var evaluator = new Evaluator(settings);
            var runner = RunnableFactory.GetInstance(settings);
            runner.InitializeRunnable();
            var analyzer = new ArimaWithRBinary(0, 2, 0);

            while (true)
            {
                var metrics = collector.Collect();

                logger.Info(metrics);

                EvaluatorAction eval = evaluator.Evaluate(analyzer.Calculate(metrics.Select(x => x.Value).ToArray()));

                if (eval == EvaluatorAction.AddResource)
                    runner.AddResource();
                else if (eval == EvaluatorAction.RemoveResource)
                    runner.RemoveResource();

                System.Threading.Thread.Sleep(15000);
            }
        }

        static ILog ConfigureLog()
        {
            var logRepo = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepo, new System.IO.FileInfo("log4net.config"));
            var logger = LogManager.GetLogger(typeof(Program));
            return logger;
        }
    }
}
