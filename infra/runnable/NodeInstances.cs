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
        public NodeInstances(Settings settings, ILog log){
            nodesavaliable = new List<string>();
            nodesactives = new List<string>();
            nodesavaliable.Add("127.0.0.1");
            nodesavaliable.Add("192.168.99.110");
            nodesactives.Add("127.0.0.1");
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
            _client = new SshClient(ip, "docker", "tcuser");
            _client.Connect();
            _client.RunCommand("docker swarm join --token SWMTKN-1-25utwt6vlq0wt6ocbdwsh0sjulxkii9n6dnr6deyytuwzlmz4x-5bpgs7zc34j6pll7wtnpxjnm0 192.168.99.1:2377");
        }
        public void RemoveNode(){
            if (nodesactives.Count == 1)
                return;
            var ipRemove = nodesactives[nodesactives.Count - 1];
            _log.Info($"Removido o nó {ipRemove}!");
            nodesactives.RemoveAt(nodesactives.Count - 1);
            _client = new SshClient(ipRemove, "docker", "tcuser");
            _client.Connect();
            _client.RunCommand("docker swarm leave");
        }

        public int InstancesCounts(){
            return nodesactives.Count;
        }


    }
}