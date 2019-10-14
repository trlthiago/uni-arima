using log4net;
using uni_arima.infra.runnable;
using uni_elastic_manager;

namespace uni_elastic_manager.infra.runnable
{
    public class RunnableFactory
    {
        public static IRunnable GetInstance(Settings settings, ILog log, NodeInstances nodes)
        {
             if (settings.IsFake)
                return new RunnableFake();

            if (settings.Runnable == "Docker")
                return new RunnableDocker(settings, log, nodes);
            return new RunnableVM(settings, log);
        }
    }
}