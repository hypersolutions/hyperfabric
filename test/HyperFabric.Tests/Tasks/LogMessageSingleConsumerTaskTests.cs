using System.Collections.Generic;
using System.Linq;
using HyperFabric.Logging;
using HyperFabric.Tasks;
using Moq;
using Xunit;

namespace HyperFabric.Tests.Tasks
{
    public class LogMessageSingleConsumerTaskTests
    {
        [Fact]
        public void WithMessage_Post_CallsEachProvider()
        {
            var providers = new List<Mock<ILogProvider>> {new Mock<ILogProvider>(), new Mock<ILogProvider>()};
            
            using (var task = new LogMessageSingleConsumerTask(providers.Select(p => p.Object)))
            {
                var message1 = new LogMessage(StageTypes.Deployment, "Something happened", LogLevelTypes.Ok);

                task.Post(message1);
            }

            providers.ForEach(p => p.Verify(e => e.Log(It.Is<string>(
                    m => m.Contains("\"Timestamp\":") && 
                         m.Contains("\"Stage\":\"Deployment\"") && 
                         m.Contains("\"LogLevel\":\"Ok\"") && 
                         m.Contains("\"Message\":\"Something happened\""))),
                Times.Once));
        }
    }
}
