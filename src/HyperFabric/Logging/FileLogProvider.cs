using System;
using System.IO;

namespace HyperFabric.Logging
{
    public sealed class FileLogProvider : ILogProvider
    {
        private readonly string _outputFile = Path.Combine(Environment.CurrentDirectory, "out.json");
        private bool _init = true;
        private bool _isDisposed;

        ~FileLogProvider()
        {
            Dispose(false);
        }
        
        public void Log(string message)
        {
            using var writer = new StreamWriter(_outputFile, !_init);
            
            if (_init)
            {
                writer.WriteLine("[");
                writer.Write(message);
                _init = false;
            }
            else
            {
                writer.Write($",{Environment.NewLine}");
                writer.Write(message);
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                using var writer = new StreamWriter(_outputFile, true);
                writer.WriteLine();
                writer.WriteLine("]");
            }
            
            _isDisposed = true;
        }
    }
}
