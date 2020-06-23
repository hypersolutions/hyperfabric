using System;

namespace HyperFabric.Integration.Tests
{
    public static class TestInfo
    {
        public static string OutputDir => AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory;

        public static string Connection => "http://localhost:19080";
    }
}
