using log4net;
using log4net.Config;
using System.Linq;
using System.Reflection;
using uni_arima.infra.evaluator;
using uni_arima.infra.runnable;
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
            var nodes = new NodeInstances(settings, logger);
            var binpacking = new BinPackingService(nodes);
            var evaluator = new Evaluator(settings);
            var runner = RunnableFactory.GetInstance(settings, logger, nodes);
            runner.InitializeRunnable();
            var analyzer = new ArimaWithRBinary(2, 1, 2, logger);
            nodes.RemoveNode();

            while (true)
            {
                var metrics = collector.Collect();

                logger.Info(metrics.Select(x => x.Value).ToArray());

                EvaluatorAction eval = evaluator.Evaluate(analyzer.Calculate(metrics.Select(x => x.Value).ToArray()));

                if (eval == EvaluatorAction.AddResource)
                    runner.AddResource();
                else if (eval == EvaluatorAction.RemoveResource)
                    runner.RemoveResource();
                
                // if (settings.BinPacking)



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
