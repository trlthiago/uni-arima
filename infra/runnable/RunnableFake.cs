using System.Threading.Tasks;

namespace uni_elastic_manager.infra.runnable
{
    public class RunnableFake : IRunnable
    {
        public void InitializeRunnable(){
            
        }
        public void AddResource()
        {
            
        }

        public async Task<RunnableState> getStateAsync()
        {
            return RunnableState.Ready;            
        }
        

        public void RemoveResource()
        {
           
        }
    }
}