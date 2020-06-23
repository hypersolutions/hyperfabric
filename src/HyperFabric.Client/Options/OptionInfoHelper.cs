using System.Linq;

namespace HyperFabric.Client.Options
{
    public sealed class OptionInfoHelper<TOptions> where TOptions : new()
    {
        private readonly CommandLineHelper<TOptions> _helper;
        private readonly string _optionName;

        public OptionInfoHelper(CommandLineHelper<TOptions> helper, string optionName)
        {
            _helper = helper;
            _optionName = optionName;
        }

        public CommandLineHelper<TOptions> IsOptional(
            string description, 
            string shortOption, 
            string longOption, 
            object defaultValue = null)
        {
            var info = _helper.Information.First(oi => oi.Name == _optionName);
            info.IsOptional = true;
            info.Description = description;
            info.ShortOption = shortOption;
            info.LongOption = longOption;
            info.DefaultValue = defaultValue;
            return _helper;
        }
        
        public CommandLineHelper<TOptions> IsRequired(
            string description, 
            string shortOption, 
            string longOption, 
            object defaultValue = null)
        {
            var info = _helper.Information.First(oi => oi.Name == _optionName);
            info.Description = description;
            info.ShortOption = shortOption;
            info.LongOption = longOption;
            info.DefaultValue = defaultValue;
            return _helper;
        }
    }
}
