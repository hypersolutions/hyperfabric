namespace HyperFabric.Client.Options
{
    public sealed class OptionInfo
    {
        public OptionInfo(string name)
        {
            Name = name;
        }
        
        public string Name { get; }
        
        public string Description { get; set; }
        
        public bool IsOptional { get; set; }
        
        public string ShortOption { get; set; }
        
        public string LongOption { get; set; }
        
        public object DefaultValue { get; set; }
    }
}
