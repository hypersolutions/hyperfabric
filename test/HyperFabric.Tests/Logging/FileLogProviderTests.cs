using System.IO;
using HyperFabric.Logging;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Logging
{
    [Collection("File-Sequential")]
    public class FileLogProviderTests
    {
        [Fact]
        public void WithMessage_Log_AddsSquareBracket()
        {
            using var logger = new FileLogProvider();
            
            logger.Log("Trying to save...");

            using var reader = new StreamReader("out.json");
            var text = reader.ReadToEnd();
            text.ShouldContain("[");
        }
        
        [Fact]
        public void Closing_Dispose_AddsSquareBracket()
        {
            using var logger = new FileLogProvider();
            
            logger.Dispose();
                
            using var reader = new StreamReader("out.json");
            var text = reader.ReadToEnd();
            text.ShouldContain("]");
        }
        
        [Fact]
        public void WithMessage_Log_WritesConsoleOutput()
        {
            using var logger = new FileLogProvider();
            
            logger.Log("Trying to save...");
                
            using var reader = new StreamReader("out.json");
            var text = reader.ReadToEnd();
            text.ShouldContain("Trying to save...");
        }
        
        [Fact]
        public void WithMessage_Log_AppendsCommaToPreviousMessage()
        {
            using var logger = new FileLogProvider();
            logger.Log("Trying to save...");
            
            logger.Log("Saved");
                
            using var reader = new StreamReader("out.json");
            var text = reader.ReadToEnd();
            text.ShouldContain("Trying to save...,");
        }
    }
}
