using System.Collections.Generic;
using log4net;
using Renci.SshNet;
using uni_elastic_manager;

namespace uni_arima.infra.runnable
{
    public class NodeInstances
    {
        private List<string> nodesavaliable;
        private List<string> nodesactives;
        private ILog _log;
        private SshClient _client;
        private Settings _settings;
        public NodeInstances(Settings settings, ILog log){
            _settings = settings;
            nodesavaliable = new List<string>();
            nodesactives = new List<string>();
            // nodesavaliable.Add("127.0.0.1");
            // nodesactives.Add("127.0.0.1");
            nodesavaliable.Add("node001");
            nodesavaliable.Add("node002");
            nodesavaliable.Add("node003");
            nodesactives.Add("node001");
            _log = log;
        }

        public void AddNode(){
            string ip = "";
            foreach (var item in nodesavaliable)
            {
                if(nodesactives.IndexOf(item) == -1){
                    ip = item;
                    break;
                }
            }
            if (ip == "")
                return;
            _log.Info($"Adicionado o nó {ip}!");
            nodesactives.Add(ip);
            _client = new SshClient(ip, _settings.SSHLogin, _settings.SSHPass);
            _client.Connect();
            _client.RunCommand(_settings.SwarmCommand);
            System.Threading.Thread.Sleep(60000);
        }
        public void RemoveNode(){
            if (nodesactives.Count == 1)
                return;
            var ipRemove = nodesactives[nodesactives.Count - 1];
            _log.Info($"Removido o nó {ipRemove}!");
            nodesactives.RemoveAt(nodesactives.Count - 1);
            _client = new SshClient(ipRemove, _settings.SSHLogin, _settings.SSHPass);
            _client.Connect();
            _client.RunCommand("docker swarm leave");
        }

        public int InstancesCounts(){
            return nodesactives.Count;
        }

        public int InstancesAvaliableCounts(){
            return nodesavaliable.Count;
        }        


    }
}