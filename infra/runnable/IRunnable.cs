using System.Threading.Tasks;

namespace uni_elastic_manager.infra.runnable
{
    public interface IRunnable
    {
         bool AddResource();
         bool RemoveResource();
         void InitializeRunnable();
         Task<RunnableState> getStateAsync();
    }
}