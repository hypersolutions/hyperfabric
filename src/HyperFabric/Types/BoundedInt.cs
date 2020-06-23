namespace HyperFabric.Types
{
    public struct BoundedInt
    {
        private int _value;
        
        public BoundedInt(int min, int max, int defaultValue)
        {
            Min = min;
            Max = max;
            DefaultValue = defaultValue;
            _value = defaultValue;
        }
        
        public int Min { get; }
        
        public int Max { get; }
        
        public int DefaultValue { get; }

        public int Value
        {
            get => _value;
            set => _value = GetValue(value);
        }

        public static implicit operator int(BoundedInt value) => value.Value;
        
        private int GetValue(int value)
        {
            return value < Min || value > Max ? DefaultValue : value;
        }
    }
}
