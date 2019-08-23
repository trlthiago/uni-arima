using uni_elastic_manager;

namespace uni_arima.infra.runnable
{
    public class RunnableFactory
    {
        public static IRunnable GetInstance(Settings settings)
        {
            if (settings.Runnable == "Docker") return new RunnableDocker(settings);
            return new RunnableVM(settings);
        }
        
    }
}