using System;
using System.Collections.Generic;
using Docker.DotNet;
using Docker.DotNet.Models;
using log4net;
using uni_arima.infra.runnable;

namespace uni_elastic_manager.infra.runnable
{
    public class RunnableDocker : IRunnable
    {
        protected readonly Settings _settings;
        private readonly DockerClient _client;
        private readonly int replicaspernode = 5;
        private ulong replicas { get; set; }
        private string IDService;
        private ILog _log;
        private NodeInstances _nodes;

        public RunnableDocker(Settings settings, ILog log, NodeInstances nodes)
        {
            _settings = settings;
            _client = new DockerClientConfiguration(new Uri(_settings.SocketDocker)).CreateClient();
            _log = log;
            _nodes = nodes;
        }

        public async void InitializeRunnable()
        {
            replicas = 10;
            var response = await _client.Swarm.CreateServiceAsync(new ServiceCreateParameters()
            {
                Service = ReturnParametersService()
            });
            IDService = response.ID;
            UpdateExporter();
        }

        public void AddResource()
        {
            replicas++;
            UpdateService();
            if ((int)replicas > (_nodes.InstancesCounts() * replicaspernode))
                AddNode();
            _log.Info("Adicionada uma replica!");
        }

        public void RemoveResource()
        {
            if (replicas == 1)
                return;
            replicas--;
            UpdateService();
            _log.Info("Removida uma replica!");
        }

        private async void UpdateService()
        {
            var service = await _client.Swarm.InspectServiceAsync(IDService);
            await _client.Swarm.UpdateServiceAsync(IDService, new ServiceUpdateParameters()
            {
                Service = ReturnParametersService(),
                Version = (long)service.Version.Index
            });
        }

        private ServiceSpec ReturnParametersService()
        {
            return new ServiceSpec()
            {
                Name = "python-app",
                Mode = new ServiceMode()
                {
                    Replicated = new ReplicatedService()
                    {
                        Replicas = replicas
                    }
                },
                TaskTemplate = new TaskSpec()
                {
                    ContainerSpec = new ContainerSpec()
                    {
                        Image = "igornardin/newtonpython:v1.0",
                    }
                },
                EndpointSpec = new EndpointSpec()
                {
                    Ports = new List<PortConfig>
                    {
                        new PortConfig()
                        {
                            Protocol = "tcp",
                            TargetPort = 80,
                            PublishedPort = 4000
                        }
                    }
                },
                Networks = new List<NetworkAttachmentConfig>
                {
                    new NetworkAttachmentConfig()
                    {
                        Target = "newtoncotes"
                    }
                }
            };
        }

        private void AddNode()
        {
            _nodes.AddNode();
            UpdateExporter();
        }

        public async void UpdateExporter()
        {
            await _client.Swarm.CreateServiceAsync(new ServiceCreateParameters()
            {
                Service = new ServiceSpec()
                {
                    Name = "exporter",
                    Mode = new ServiceMode()
                    {
                        Replicated = new ReplicatedService()
                        {
                            Replicas = (ulong) _nodes.InstancesCounts()
                        }
                    },
                    TaskTemplate = new TaskSpec()
                    {
                        ContainerSpec = new ContainerSpec()
                        {
                            Image = "wywywywy/docker_stats_exporter:latest",
                            Mounts = new List<Mount>
                            {
                                new Mount
                                {
                                    Source = "/var/run/docker.sock",
                                    Target = "/var/run/docker.sock"
                                },
                                new Mount
                                {
                                    Source = "/usr/bin/docker",
                                    Target = "/usr/bin/docker"
                                }
                            }
                        }
                    },
                    EndpointSpec = new EndpointSpec()
                    {
                        Ports = new List<PortConfig>
                        {
                            new PortConfig(){
                                Protocol = "tcp",
                                TargetPort = 9487,
                                PublishedPort = 9487
                            }

                        }
                    }
                }
            });
        }
    }
}