using System.Threading.Tasks;

namespace uni_elastic_manager.infra.runnable
{
    public class RunnableFake : IRunnable
    {
        public void InitializeRunnable(){
            
        }
        public bool AddResource()
        {
            return false;            
        }

        public async Task<RunnableState> getStateAsync()
        {
            return RunnableState.Ready;            
        }
        

        public bool RemoveResource()
        {
            return false;
        }
    }
}