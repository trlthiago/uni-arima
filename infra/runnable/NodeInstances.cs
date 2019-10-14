using System.Collections.Generic;
using log4net;
using uni_elastic_manager;

namespace uni_arima.infra.runnable
{
    public class NodeInstances
    {
        private List<string> nodesavaliable;
        private List<string> nodesactives;
        private ILog _log;
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
            _log.Info("Removido um novo nó!");
            nodesactives.RemoveAt(nodesactives.Count - 1);
        }

        public int InstancesCounts(){
            return nodesactives.Count;
        }


    }
}