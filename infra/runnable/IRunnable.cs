using System.Threading.Tasks;

namespace uni_elastic_manager.infra.runnable
{
    public interface IRunnable
    {
         void AddResource();
         void RemoveResource();
         void InitializeRunnable();
         Task<RunnableState> getStateAsync();
    }
}