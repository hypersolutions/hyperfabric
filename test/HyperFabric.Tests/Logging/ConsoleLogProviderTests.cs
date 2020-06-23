using System;
using System.IO;
using HyperFabric.Logging;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Logging
{
    public class ConsoleLogProviderTests
    {
        [Fact]
        public void WithMessage_Log_AddsSquareBracket()
        {
            try
            {
                using var writer = new StringWriter();
                Console.SetOut(writer);
            
                using var logger = new ConsoleLogProvider();
            
                logger.Log("Trying to save...");
                
                var text = writer.ToString();
                text.ShouldContain("[");
            }
            finally
            {
                var standardOutput = new StreamWriter(Console.OpenStandardOutput()) {AutoFlush = true};
                Console.SetOut(standardOutput);
            }
        }
        
        [Fact]
        public void Closing_Dispose_AddsSquareBracket()
        {
            try
            {
                using var writer = new StringWriter();
                Console.SetOut(writer);
                var logger = new ConsoleLogProvider();
            
                logger.Dispose();
                
                var text = writer.ToString();
                text.ShouldContain("]");
            }
            finally
            {
                var standardOutput = new StreamWriter(Console.OpenStandardOutput()) {AutoFlush = true};
                Console.SetOut(standardOutput);
            }
        }
        
        [Fact]
        public void WithMessage_Log_WritesConsoleOutput()
        {
            try
            {
                using var writer = new StringWriter();
                Console.SetOut(writer);
                using var logger = new ConsoleLogProvider();
            
                logger.Log("Trying to save...");
                
                var text = writer.ToString();
                text.ShouldContain("Trying to save...");
            }
            finally
            {
                var standardOutput = new StreamWriter(Console.OpenStandardOutput()) {AutoFlush = true};
                Console.SetOut(standardOutput);
            }
        }
        
        [Fact]
        public void WithMessage_Log_AppendsCommaToPreviousMessage()
        {
            try
            {
                using var writer = new StringWriter();
                Console.SetOut(writer);
                using var logger = new ConsoleLogProvider();
                logger.Log("Trying to save...");
            
                logger.Log("Saved");
                
                var text = writer.ToString();
                text.ShouldContain("Trying to save...,");
            }
            finally
            {
                var standardOutput = new StreamWriter(Console.OpenStandardOutput()) {AutoFlush = true};
                Console.SetOut(standardOutput);
            }
        }
    }
}
