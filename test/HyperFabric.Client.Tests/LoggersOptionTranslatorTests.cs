using Shouldly;
using Xunit;

namespace HyperFabric.Client.Tests
{
    public class LoggersOptionTranslatorTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidLoggers_Translate_ReturnsEmptyList(string loggers)
        {
            var translator = new LoggersOptionTranslator();
            
            var result = translator.Translate(loggers);
            
            result.ShouldBeEmpty();
        }
        
        [Theory]
        [InlineData("Console")]
        [InlineData(" Console")]
        [InlineData(" Console ")]
        [InlineData(" , Console, ")]
        public void SingleLogger_Translate_ReturnsSingleItemList(string loggers)
        {
            var translator = new LoggersOptionTranslator();

            var result = translator.Translate(loggers);
            
            result.Length.ShouldBe(1);
            result.ShouldContain("Console");
        }
        
        [Theory]
        [InlineData("Console,File")]
        [InlineData(" File, Console")]
        [InlineData(" Console , File ")]
        [InlineData(" , Console, File , ")]
        public void MultipleLoggers_Translate_ReturnsMultipleItemList(string loggers)
        {
            var translator = new LoggersOptionTranslator();
            
            var result = translator.Translate(loggers);
            
            result.Length.ShouldBe(2);
            result.ShouldContain("Console");
            result.ShouldContain("File");
        }
    }
}
