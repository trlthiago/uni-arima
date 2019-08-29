using System;
using System.Collections.Generic;
using Docker.DotNet;
using Docker.DotNet.Models;
using uni_elastic_manager;

namespace uni_arima.infra.runnable
{
    public class RunnableDocker : IRunnable
    {
        protected readonly Settings _settings;
        protected readonly double _CPUThresholdUpper;
        protected readonly double _CPUThresholdLower;
        private readonly DockerClient _client;

        public RunnableDocker(Settings settings)
        {
            _settings = settings;
            _CPUThresholdUpper = Convert.ToDouble(_settings.CPUThresholdUpper);
            _CPUThresholdLower = Convert.ToDouble(_settings.CPUThresholdLower);
            _client = new DockerClientConfiguration(new Uri(_settings.SocketDocker)).CreateClient();
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


        protected async void AddContainer()
        {
            var containers = await _client.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    Limit = 10,
                });
            foreach(var container in containers){
                Console.WriteLine($"Container ID: {container.ID}");
            }

                       
        }

        protected async void RemoveContainer()
        {
            var nodes = await _client.Swarm.ListNodesAsync();
            foreach(var node in nodes){
                
                Console.WriteLine($"Container ID: {node.ID} Container Image: {node.Status.State}");
            }
        }
    }
}