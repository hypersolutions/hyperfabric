using System.Linq;
using HyperFabric.Factories;
using HyperFabric.Logging;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Factories
{
    [Collection("File-Sequential")]
    public class LoggingFactoryTests
    {
        [Fact]
        public void UnknownProvider_Create_ReturnsSingleProviderCount()
        {
            var factory = new LoggingFactory();

            using var logger = factory.Create(new []{"Unknown"});
            
            logger.Providers.Count().ShouldBe(1);
        }
        
        [Fact]
        public void UnknownProvider_Create_ReturnsDefaultConsoleLogProvider()
        {
            var factory = new LoggingFactory();

            using var logger = factory.Create(new []{"Unknown"});
            
            logger.Providers.First().ShouldBeOfType<ConsoleLogProvider>();
        }
        
        [Fact]
        public void EmptyProviderList_Create_ReturnsDefaultConsoleLogProvider()
        {
            var factory = new LoggingFactory();

            using var logger = factory.Create(new string[0]);
            
            logger.Providers.First().ShouldBeOfType<ConsoleLogProvider>();
        }
        
        [Theory]
        [InlineData("Console")]
        [InlineData("console")]
        [InlineData("CONSOLE")]
        public void ConsoleOnly_Create_ReturnsConsoleLogProvider(string loggerName)
        {
            var factory = new LoggingFactory();

            using var logger = factory.Create(new[]{loggerName});
            
            logger.Providers.First().ShouldBeOfType<ConsoleLogProvider>();
        }
        
        [Theory]
        [InlineData("File")]
        [InlineData("file")]
        [InlineData("FILE")]
        public void FileOnly_Create_ReturnsFileLogProvider(string loggerName)
        {
            var factory = new LoggingFactory();

            using var logger = factory.Create(new[]{loggerName});
            
            logger.Providers.First().ShouldBeOfType<FileLogProvider>();
        }
        
        [Fact]
        public void ConsoleAndFile_Create_ReturnsMultipleProvidersCount()
        {
            var factory = new LoggingFactory();

            using var logger = factory.Create(new[]{"Console", "File"});
            
            logger.Providers.Count().ShouldBe(2);
        }
        
        [Fact]
        public void ConsoleAndFile_Create_ReturnsMultipleProviders()
        {
            var factory = new LoggingFactory();

            using var logger = factory.Create(new[]{"Console", "File"});
            
            logger.Providers.First().ShouldBeOfType<ConsoleLogProvider>();
            logger.Providers.Last().ShouldBeOfType<FileLogProvider>();
        }

        [Fact]
        public void FileAndConsole_Create_ReturnsMultipleProviders()
        {
            var factory = new LoggingFactory();

            using var logger = factory.Create(new[]{"File", "Console"});
            
            logger.Providers.First().ShouldBeOfType<FileLogProvider>();
            logger.Providers.Last().ShouldBeOfType<ConsoleLogProvider>();
        }
    }
}
