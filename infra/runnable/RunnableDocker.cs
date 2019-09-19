using System;
using System.Collections.Generic;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace uni_elastic_manager.infra.runnable
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
            var response = await _client.Containers.CreateContainerAsync
            (
                new CreateContainerParameters()
                {
                    Image = "igornardin/newtonpython:v1.0"
                }
            );
            await _client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());
            Console.WriteLine($"Iniciado o container ID {response.ID}");
        }

        public async void RemoveResource()
        {
            var containers = await _client.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                    {
                        {
                            "status", new Dictionary<string, bool> { {"running", true} }
                        }
                    }
                }
            );
            foreach (var container in containers)
            {
                if (container.Image == _settings.Image)
                {
                    await _client.Containers.StopContainerAsync(container.ID, new ContainerStopParameters());
                    await _client.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters());
                    Console.WriteLine($"Removido o container ID {container.ID}");
                    break;
                }
            }
        }
    }
}