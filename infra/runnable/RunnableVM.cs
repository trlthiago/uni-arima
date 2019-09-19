using System;
using uni_elastic_manager;

namespace uni_elastic_manager.infra.runnable
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

        public void AddResource()
        {
             throw new System.NotImplementedException();
        }

        public void RemoveResource()
        {
            throw new System.NotImplementedException();
        }
    }
}