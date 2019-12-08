using System;
using System.Collections.Generic;
using System.IO;
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
        Finished = 3,
    }

    public class RunnableDocker : IRunnable
    {
        protected readonly Settings _settings;
        private readonly DockerClient _client;
        private readonly int replicaspernode = 8;
        private readonly int replicasperadd = 1;
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
            var services = await _client.Swarm.ListServicesAsync();
            foreach (var item in services)
            {
                await _client.Swarm.RemoveServiceAsync(item.ID);
            }
            replicas = (ulong) replicasperadd;
            var response = await _client.Swarm.CreateServiceAsync(new ServiceCreateParameters()
            {
                Service = ReturnParametersService()
            });
            IDService = response.ID;
            File.Delete("/home/igorn/workspace/finalizou.txt");
        }

        public async Task<RunnableState> getStateAsync()
        {
            if (File.Exists("/home/igorn/workspace/finalizou.txt")){
                return RunnableState.Finished;
            }
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


        public bool AddResource()
        {
            if (replicas == (ulong) replicaspernode * (ulong) _nodes.InstancesAvaliableCounts())
                return false;
            _log.Info($"Adicionada {replicasperadd} replica!");
            replicas = replicas + (ulong) replicasperadd;
            if (((int)replicas > (_nodes.InstancesCounts() * replicaspernode)) && (_nodes.InstancesCounts() < _nodes.InstancesAvaliableCounts()))
            {
                AddNode();
                UpdateService();
                return true;
            }
            UpdateService();
            return false;
        }

        public bool RemoveResource()
        {
            if (replicas == (ulong) replicasperadd)
                return false;
            _log.Info($"Removida {replicasperadd} replica!");
            replicas = replicas - (ulong) replicasperadd;
            UpdateService();
            if (_nodes.InstancesCounts() > Math.Ceiling( (double) replicas / replicaspernode))
            {
                RemoveNode();
                return true;
            }
            return false;
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
                        Image = "igornardin/newtonpython:v2.0",

                    },
                    Placement = new Placement()
                    {
                        Constraints = new List<string>()
                        {
                            "node.role == worker"
                        }
                    },
                    Resources = new ResourceRequirements(){
                        Limits = new SwarmResources(){
                            NanoCPUs = 1000000000
                        }
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
                            PublishedPort = 4000,
                            PublishMode = "ingress"
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
        }
        private void RemoveNode()
        {
            _nodes.RemoveNode();
        }
    }
}