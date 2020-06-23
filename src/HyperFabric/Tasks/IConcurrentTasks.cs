using System.Threading.Tasks;

namespace HyperFabric.Tasks
{
    public interface IConcurrentTasks
    {
        Task StartAsync();
    }
}
