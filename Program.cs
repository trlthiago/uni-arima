using log4net;
using log4net.Config;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using uni_arima.infra.evaluator;
using uni_arima.infra.runnable;
using uni_elastic_manager.infra;
using uni_elastic_manager.infra.runnable;

namespace uni_elastic_manager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            configure();
            var logger = ConfigureLog();
            var settings = new Settings();
            var collector = PrometheusFactory.GetInstance(settings);
            var nodes = new NodeInstances(settings, logger);
            var runner = RunnableFactory.GetInstance(settings, logger, nodes);
            var evaluator = new Evaluator(settings);
            runner.InitializeRunnable();
            var analyzer = new ArimaWithRBinary(2, 1, 2, logger);
            var state = runner.getStateAsync();         

            while (true)
            {
                if(await runner.getStateAsync() == RunnableState.Busy)
                {
                    continue;
                }
                var metrics = collector.Collect();

                logger.Info(metrics.Select(x => x.Value).ToArray());

                EvaluatorAction eval = evaluator.Evaluate(analyzer.Calculate(metrics.Select(x => x.Value).ToArray()));

                if (eval == EvaluatorAction.AddResource)
                {
                    runner.AddResource();
                    collector.ResetStartTime();
                }
                else if (eval == EvaluatorAction.RemoveResource)
                {
                    runner.RemoveResource();
                    collector.ResetStartTime();
                }               
                System.Threading.Thread.Sleep(15000);
            }
        }
        static void configure(){

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
