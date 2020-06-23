using HyperFabric.Client.Options;
using Shouldly;
using Xunit;

namespace HyperFabric.Client.Tests.Options
{
    public class LoggersOptionHandlerTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidLoggers_Handle_SetsLoggerListAsEmpty(string loggers)
        {
            var handler = new LoggersOptionHandler();
            var options = new DeploymentOptions {Loggers = loggers};
            
            handler.Handle(options);
            
            options.LoggerList.ShouldBeEmpty();
        }
        
        [Theory]
        [InlineData("Console")]
        [InlineData(" Console")]
        [InlineData(" Console ")]
        [InlineData(" , Console, ")]
        public void SingleLogger_Handle_SetsLoggerListWithOneEntry(string loggers)
        {
            var handler = new LoggersOptionHandler();
            var options = new DeploymentOptions {Loggers = loggers};
            
            handler.Handle(options);
            
            options.LoggerList.Length.ShouldBe(1);
            options.LoggerList.ShouldContain("Console");
        }
        
        [Theory]
        [InlineData("Console,File")]
        [InlineData(" File, Console")]
        [InlineData(" Console , File ")]
        [InlineData(" , Console, File , ")]
        public void MultipleLoggers_Handle_SetsLoggerListAsConsole(string loggers)
        {
            var handler = new LoggersOptionHandler();
            var options = new DeploymentOptions {Loggers = loggers};
            
            handler.Handle(options);
            
            options.LoggerList.Length.ShouldBe(2);
            options.LoggerList.ShouldContain("Console");
            options.LoggerList.ShouldContain("File");
        }
    }
}
