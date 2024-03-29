using System;
using System.Threading.Tasks;
using log4net;
using uni_elastic_manager;

namespace uni_elastic_manager.infra.runnable
{
    public class RunnableVM : IRunnable
    {
        protected readonly Settings _settings;
        protected readonly double _CPUThresholdUpper;
        protected readonly double _CPUThresholdLower;
        private ILog _log;

        public RunnableVM(Settings settings, ILog log)
        {
            _settings = settings;
            _CPUThresholdUpper = Convert.ToDouble(_settings.CPUThresholdUpper);
            _CPUThresholdLower = Convert.ToDouble(_settings.CPUThresholdLower);
            _log = log;
        }
        public void InitializeRunnable(){
            
        }
        public async Task<RunnableState> getStateAsync()
        {
            return RunnableState.Ready;            
        }


        public bool AddResource()
        {
             throw new System.NotImplementedException();
        }

        public bool RemoveResource()
        {
            throw new System.NotImplementedException();
        }
    }
}