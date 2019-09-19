using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Docker.DotNet;
using Docker.DotNet.Models;
using uni_elastic_manager;

namespace uni_arima.infra.runnable
{
    public class RunnableDocker : IRunnable
    {
        protected readonly Settings _settings;
        private readonly DockerClient _client;

        public RunnableDocker(Settings settings)
        {
            _settings = settings;
            _client = new DockerClientConfiguration(new Uri(_settings.SocketDocker)).CreateClient();
        }

        public async void AddResource()
        {
            var containers = await _client.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    Limit = 10,
                });
            foreach (var container in containers)
            {
                Console.WriteLine($"Container ID: {container.ID}");
            }


        }

        public async void RemoveResource()
        {
            var nodes = await _client.Swarm.ListNodesAsync();
            foreach (var node in nodes)
            {
                Console.WriteLine($"Node ID: {node.ID} Node State: {node.Status.State}");
            }
            var containers = await _client.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                        {
                            {"status", new Dictionary<string, bool>
                                {
                                    {"running", true}
                                }
                            }
                        }
                });
            foreach (var container in containers)
            {
                if (container.Image == _settings.Image)
                {
                    Console.WriteLine($"Container ID: {container.ID} Container Image: {container.Image} Container state:{container.State}");
                }
            }
        }
    }
}