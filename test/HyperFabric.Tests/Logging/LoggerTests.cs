using System.Collections.Generic;
using System.Linq;
using HyperFabric.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Logging
{
    public class LoggerTests
    {
        [Fact]
        public void SingleMessageAndProvider_Log_CallsProviderLog()
        {
            var providers = new List<Mock<ILogProvider>> {new Mock<ILogProvider>()};
            
            using (var logger = new Logger(providers.Select(p => p.Object)))
            {
                var message = new LogMessage(StageTypes.Deployment, "Something happened", LogLevelTypes.Ok);

                logger.Log(message);
            }

            providers.ForEach(p => p.Verify(e => e.Log(It.Is<string>(
                m => m.Contains("\"Timestamp\":") && 
                     m.Contains("\"Stage\":\"Deployment\"") && 
                     m.Contains("\"LogLevel\":\"Ok\"") && 
                     m.Contains("\"Message\":\"Something happened\""))),
                Times.Once));
        }
        
        [Fact]
        public void MultipleMessagesAndProvider_Log_CallsProviderLog()
        {
            var providers = new List<Mock<ILogProvider>> {new Mock<ILogProvider>()};
            
            using (var logger = new Logger(providers.Select(p => p.Object)))
            {
                var message1 = new LogMessage(StageTypes.Deployment, "Something happened", LogLevelTypes.Ok);
                var message2 = new LogMessage(StageTypes.Deployment, "Something else happened", LogLevelTypes.Ok);

                logger.Log(message1, message2);
            }

            providers.ForEach(p => p.Verify(e => e.Log(It.Is<string>(
                    m => m.Contains("\"Timestamp\":") && 
                         m.Contains("\"Stage\":\"Deployment\"") && 
                         m.Contains("\"LogLevel\":\"Ok\"") && 
                         m.Contains("\"Message\":\"Something happened\""))),
                Times.Once));
            providers.ForEach(p => p.Verify(e => e.Log(It.Is<string>(
                    m => m.Contains("\"Timestamp\":") && 
                         m.Contains("\"Stage\":\"Deployment\"") && 
                         m.Contains("\"LogLevel\":\"Ok\"") && 
                         m.Contains("\"Message\":\"Something else happened\""))),
                Times.Once));
        }
        
        [Fact]
        public void MultipleMessagesAndProviders_Log_CallsProviderLog()
        {
            var providers = new List<Mock<ILogProvider>> {new Mock<ILogProvider>(), new Mock<ILogProvider>()};
           
            using (var logger = new Logger(providers.Select(p => p.Object)))
            {
                var message1 = new LogMessage(StageTypes.Deployment, "Something happened", LogLevelTypes.Ok);
                var message2 = new LogMessage(StageTypes.Deployment, "Something else happened", LogLevelTypes.Ok);

                logger.Log(message1, message2);
            }

            providers.ForEach(p => p.Verify(e => e.Log(It.Is<string>(
                    m => m.Contains("\"Timestamp\":") && 
                         m.Contains("\"Stage\":\"Deployment\"") && 
                         m.Contains("\"LogLevel\":\"Ok\"") && 
                         m.Contains("\"Message\":\"Something happened\""))),
                Times.Once));
            providers.ForEach(p => p.Verify(e => e.Log(It.Is<string>(
                    m => m.Contains("\"Timestamp\":") && 
                         m.Contains("\"Stage\":\"Deployment\"") && 
                         m.Contains("\"LogLevel\":\"Ok\"") && 
                         m.Contains("\"Message\":\"Something else happened\""))),
                Times.Once));
        }
        
        [Fact]
        public void NoErrorMessages_HasError_ReturnsFalse()
        {
            var providers = new List<Mock<ILogProvider>> {new Mock<ILogProvider>(), new Mock<ILogProvider>()};
            using var logger = new Logger(providers.Select(p => p.Object));
            var message1 = new LogMessage(StageTypes.Deployment, "Something happened", LogLevelTypes.Ok);
            var message2 = new LogMessage(StageTypes.Deployment, "Something else happened", LogLevelTypes.Ok);
            logger.Log(message1, message2);

            logger.HasErrors.ShouldBeFalse();
        }
        
        [Fact]
        public void ErrorMessages_HasError_ReturnsTrue()
        {
            var providers = new List<Mock<ILogProvider>> {new Mock<ILogProvider>(), new Mock<ILogProvider>()};
            using var logger = new Logger(providers.Select(p => p.Object));
            var message1 = new LogMessage(StageTypes.Deployment, "Something happened", LogLevelTypes.Ok);
            var message2 = new LogMessage(StageTypes.Deployment, "Something else happened", LogLevelTypes.Error);
            logger.Log(message1, message2);

            logger.HasErrors.ShouldBeTrue();
        }
        
        [Fact]
        public void MultipleProviders_Dispose_CallsProvidersDispose()
        {
            var providers = new List<Mock<ILogProvider>> {new Mock<ILogProvider>(), new Mock<ILogProvider>()};
            var logger = new Logger(providers.Select(p => p.Object));
            
            logger.Dispose();

            providers.ForEach(p => p.Verify(e => e.Dispose(), Times.Once));
        }
    }
}
