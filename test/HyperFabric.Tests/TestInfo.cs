using System;

namespace HyperFabric.Tests
{
    public static class TestInfo
    {
        public static string OutputDir => AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory;
    }
}
