using log4net;
using log4net.Config;
using System.Linq;
using System.Reflection;
using uni_arima.infra.runnable;
using uni_elastic_manager.infra;
using uni_elastic_manager.Infra;

namespace uni_elastic_manager
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = ConfigureLog();

            /* EXEMPLO DE USO DO LOGGER */
            logger.Error("TRL_ERROR");
            logger.Info("3.333");
            /* FIM DO EXEMPLO */

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

        static ILog ConfigureLog()
        {
            var logRepo = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepo, new System.IO.FileInfo("log4net.config"));
            var logger = LogManager.GetLogger(typeof(Program));
            return logger;
        }
    }
}
