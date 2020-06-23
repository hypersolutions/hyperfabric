using System.Threading.Tasks;

namespace HyperFabric.Commands
{
    public interface ICommand
    {
        Task RunAsync();
    }
}
