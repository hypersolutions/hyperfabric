using HyperFabric.Commands;
using HyperFabric.Logging;
using Microsoft.ServiceFabric.Client;
using Moq;

namespace HyperFabric.Tests.Commands
{
    public abstract class CommandTestBase
    {
        protected CommandTestBase()
        {
            FabricClient = new Mock<IServiceFabricClient>();
            Logger = new Mock<ILogger>();
            InnerCommand = new Mock<ICommand>();
        }

        protected Mock<IServiceFabricClient> FabricClient { get; }
        
        protected Mock<ILogger> Logger { get; }
        
        protected Mock<ICommand> InnerCommand { get; }
        
        protected void ShouldContainsLogMessage(string message)
        {
            Logger.Verify(l => l.Log(It.Is<LogMessage>(m => m.Message.Contains(message))), Times.Once);
        }
    }
}
