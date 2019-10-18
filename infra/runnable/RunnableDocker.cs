using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using log4net;
using uni_arima.infra.runnable;

namespace uni_elastic_manager.infra.runnable
{

    public enum RunnableState
    {
        Ready = 1,
        Busy = 2,
    }

    public class RunnableDocker : IRunnable
    {
        protected readonly Settings _settings;
        private readonly DockerClient _client;
        private readonly int replicaspernode = 5;
        private ulong replicas { get; set; }
        private string IDService;
        private string IDExporterService;
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
            replicas = 1;
            var response = await _client.Swarm.CreateServiceAsync(new ServiceCreateParameters()
            {
                Service = ReturnParametersService()
            });
            IDService = response.ID;
            //CreateExporter();
        }

        public async Task<RunnableState> getStateAsync()
        {
            var tasks = await _client.Tasks.ListAsync();
            foreach (var item in tasks)
            {
                if (item.ServiceID == IDService && item.Status.State != TaskState.Running)
                {
                    return RunnableState.Busy;
                }
            }
            return RunnableState.Ready;            
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
            if (_nodes.InstancesCounts() > Math.Ceiling( (double) replicas / replicaspernode))
                RemoveNode();
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
            //UpdateExporter();
        }
        private void RemoveNode()
        {
            _nodes.RemoveNode();
            //UpdateExporter();
        }
        public async void UpdateExporter()
        {
            var service = await _client.Swarm.InspectServiceAsync(IDExporterService);
            await _client.Swarm.UpdateServiceAsync(IDExporterService, new ServiceUpdateParameters()
            {
                Service = GetExporterService(),
                Version = (long)service.Version.Index
            });
        }

        public async void CreateExporter()
        {
            var response = await _client.Swarm.CreateServiceAsync(new ServiceCreateParameters()
            {
                Service = GetExporterService()
            });
            IDExporterService = response.ID;
        }

        private ServiceSpec GetExporterService()
        {
            return new ServiceSpec()
            {
                Name = "exporter",
                Mode = new ServiceMode()
                {
                    Replicated = new ReplicatedService()
                    {
                        Replicas = (ulong)_nodes.InstancesCounts()
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
            };
        }

    }
}