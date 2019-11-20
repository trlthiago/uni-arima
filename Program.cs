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
            var logger = ConfigureLog();
            var settings = new Settings();
            var collector = PrometheusFactory.GetInstance(settings);
            var nodes = new NodeInstances(settings, logger);
            var runner = RunnableFactory.GetInstance(settings, logger, nodes);
            var evaluator = new Evaluator(settings);
            runner.InitializeRunnable();
            var analyzer = new ArimaWithRBinary(2, 0, 1, logger);
            var state = runner.getStateAsync();  
            logger.Debug("Teste");

            while (true)
            {
                if(await runner.getStateAsync() == RunnableState.Busy)
                {
                    continue;
                }
                logger.Debug("Vai fazer a coleta...");
                var metrics = collector.Collect();

                logger.Info(metrics.Select(x => x.Value).ToArray());
                logger.Debug("Coleta realizada...");

                EvaluatorAction eval = evaluator.Evaluate(analyzer.Calculate(metrics.Select(x => x.Value).ToArray()));
                logger.Debug("Avalia o resultado...");
                if (eval == EvaluatorAction.AddResource)
                {
                    logger.Debug("Adiciona recurso...");
                    if (runner.AddResource())
                        collector.ResetStartTime();
                }
                else if (eval == EvaluatorAction.RemoveResource)
                {
                    logger.Debug("Remove recurso...");
                    if (runner.RemoveResource())
                        collector.ResetStartTime();
                }               
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
