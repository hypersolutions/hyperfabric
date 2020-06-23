using System.Linq;
using HyperFabric.Client.Options;
using Shouldly;
using Xunit;

namespace HyperFabric.Client.Tests.Options
{
    public class CommandLineHelperTests
    {
        [Theory]
        [InlineData("-j")]
        [InlineData("--json")]
        public void WithArgs_IsOptional_SetsIsOptionalTrue(string arg)
        {
            var args = new[] {arg};
            
            var helper = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsOptional("Path to the files.", "-j", "--json");
            
            var info = helper.Information.First(i => i.Name == "Json");
            info.IsOptional.ShouldBeTrue();
        }
        
        [Theory]
        [InlineData("-j")]
        [InlineData("--json")]
        public void WithArgs_IsOptional_SetsDescription(string arg)
        {
            var args = new[] {arg};
            
            var helper = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsOptional("Path to the files.", "-j", "--json");

            var info = helper.Information.First(i => i.Name == "Json");
            info.Description.ShouldBe("Path to the files.");
        }
        
        [Theory]
        [InlineData("-j")]
        [InlineData("--json")]
        public void WithArgs_IsOptional_SetsShortOption(string arg)
        {
            var args = new[] {arg};
            
            var helper = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsOptional("Path to the files.", "-j", "--json");

            var info = helper.Information.First(i => i.Name == "Json");
            info.ShortOption.ShouldBe("-j");
        }
        
        [Theory]
        [InlineData("-j")]
        [InlineData("--json")]
        public void WithArgs_IsOptional_SetsLongOption(string arg)
        {
            var args = new[] {arg};
            
            var helper = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsOptional("Path to the files.", "-j", "--json");

            var info = helper.Information.First(i => i.Name == "Json");
            info.LongOption.ShouldBe("--json");
        }
        
        [Fact]
        public void WithNoDefault_IsOptional_SetsDefaultValue()
        {
            var args = new[] {"-j"};
            
            var helper = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsOptional("Path to the files.", "-j", "--json");

            var info = helper.Information.First(i => i.Name == "Json");
            info.DefaultValue.ShouldBeNull();
        }
        
        [Fact]
        public void WithDefault_IsOptional_SetsDefaultValue()
        {
            var args = new[] {"-j"};
            
            var helper = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsOptional("Path to the files.", "-j", "--json",@"c:\temp");
            
            var info = helper.Information.First(i => i.Name == "Json");
            info.DefaultValue.ShouldBe(@"c:\temp");
        }
        
        [Theory]
        [InlineData("-j")]
        [InlineData("--json")]
        public void WithArgs_IsRequired_SetsIsRequiredTrue(string arg)
        {
            var args = new[] {arg};
            
            var helper = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsRequired("Path to the files.", "-j", "--json");
            
            var info = helper.Information.First(i => i.Name == "Json");
            info.IsOptional.ShouldBeFalse();
        }
        
        [Theory]
        [InlineData("-j")]
        [InlineData("--json")]
        public void WithArgs_IsRequired_SetsDescription(string arg)
        {
            var args = new[] {arg};
            
            var helper = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsRequired("Path to the files.", "-j", "--json");

            var info = helper.Information.First(i => i.Name == "Json");
            info.Description.ShouldBe("Path to the files.");
        }
        
        [Theory]
        [InlineData("-j")]
        [InlineData("--json")]
        public void WithArgs_IsRequired_SetsShortOption(string arg)
        {
            var args = new[] {arg};
            
            var helper = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsRequired("Path to the files.", "-j", "--json");

            var info = helper.Information.First(i => i.Name == "Json");
            info.ShortOption.ShouldBe("-j");
        }
        
        [Theory]
        [InlineData("-j")]
        [InlineData("--json")]
        public void WithArgs_IsRequired_SetsLongOption(string arg)
        {
            var args = new[] {arg};
            
            var helper = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsRequired("Path to the files.", "-j", "--json");

            var info = helper.Information.First(i => i.Name == "Json");
            info.LongOption.ShouldBe("--json");
        }
        
        [Fact]
        public void WithNoDefault_IsRequired_SetsDefaultValue()
        {
            var args = new[] {"-j"};
            
            var helper = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsRequired("Path to the files.", "-j", "--json");

            var info = helper.Information.First(i => i.Name == "Json");
            info.DefaultValue.ShouldBeNull();
        }
        
        [Fact]
        public void WithDefault_IsRequired_SetsDefaultValue()
        {
            var args = new[] {"-j"};
            
            var helper = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsRequired("Path to the files.", "-j", "--json", @"c:\temp");
            
            var info = helper.Information.First(i => i.Name == "Json");
            info.DefaultValue.ShouldBe(@"c:\temp");
        }
        
        [Fact]
        public void WithHandlerAndJsonString_Parse_SetsJsonString()
        {
            var args = new[] {"-j", "{}"};
            
            var options = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsRequired("Path to the files.", "-j", "--json")
                .WithOptionHandler(new JsonOptionHandler())
                .Parse();
            
            options.JsonString.ShouldBe("{}");
        }
        
        [Fact]
        public void WithHandlerAndJsonPath_Parse_SetsJsonPath()
        {
            var args = new[] {"-j", @"c:\temp"};
            
            var options = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "Test", "Description")
                .For(a => a.Json)
                .IsRequired("Path to the files.", "-j", "--json")
                .WithOptionHandler(new JsonOptionHandler())
                .Parse();
            
            options.JsonPath.ShouldBe(@"c:\temp");
        }
    }
}
