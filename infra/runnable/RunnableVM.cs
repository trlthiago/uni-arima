using System;
using uni_elastic_manager;

namespace uni_arima.infra.runnable
{
    public class RunnableVM : IRunnable
    {
        protected readonly Settings _settings;
        protected readonly double _CPUThresholdUpper;
        protected readonly double _CPUThresholdLower;

        public RunnableVM(Settings settings)
        {
            _settings = settings;
            _CPUThresholdUpper = Convert.ToDouble(_settings.CPUThresholdUpper);
            _CPUThresholdLower = Convert.ToDouble(_settings.CPUThresholdLower);
        }
        public bool Evaluate(double value)
        {
            throw new System.NotImplementedException();
        }

        protected static void AddContainer()
        {
             throw new System.NotImplementedException();
        }

        protected static void RemoveContainer()
        {
            throw new System.NotImplementedException();
        }
    }
}