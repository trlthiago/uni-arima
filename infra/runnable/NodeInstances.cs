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
            nodesactives.Add("192.168.99.110");
            _log = log;
        }

        public void AddNode(){
            _log.Info("Adicionado um novo nó!");
            string id = "";
            foreach (var item in nodesavaliable)
            {
                if(nodesactives.IndexOf(item) == -1){
                    id = item;
                    break;
                }
            }
            if (id == "")
                return;
            nodesactives.Add(id);
        }
        public void RemoveNode(){
            var ipRemove = nodesactives[nodesactives.Count - 1];
            _log.Info($"Removido o nó {ipRemove}!");
            nodesactives.RemoveAt(nodesactives.Count - 1);
            _client = new SshClient(ipRemove, "docker", "tcuser");
            _client.Connect();
            _client.RunCommand("sudo poweroff");
        }

        public int InstancesCounts(){
            return nodesactives.Count;
        }


    }
}