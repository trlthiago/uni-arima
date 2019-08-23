using System;
using Docker.DotNet;
using uni_elastic_manager;

namespace uni_arima.infra.runnable
{
    public class RunnableDocker : IRunnable
    {
        protected readonly Settings _settings;
        protected readonly double _CPUThresholdUpper;
        protected readonly double _CPUThresholdLower;
        protected readonly DockerClient client;

        public RunnableDocker(Settings settings)
        {
            _settings = settings;
            _CPUThresholdUpper = Convert.ToDouble(_settings.CPUThresholdUpper);
            _CPUThresholdLower = Convert.ToDouble(_settings.CPUThresholdLower);
            client = new DockerClientConfiguration(new Uri("http://localhost:4243")).CreateClient();
        }
        public bool Evaluate(double value)
        {
            if (value > _CPUThresholdUpper)
            {
                AddContainer();
                return true;
            }
            else if (value < _CPUThresholdLower)
            {
                RemoveContainer();
                return true;
            }
            return false;
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