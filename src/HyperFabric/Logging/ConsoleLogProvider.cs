using System;

namespace HyperFabric.Logging
{
    public sealed class ConsoleLogProvider : ILogProvider
    {
        private bool _init = true;
        private bool _isDisposed;

        ~ConsoleLogProvider()
        {
            Dispose(false);
        }
        
        public void Log(string message)
        {
            if (_init)
            {
                Console.WriteLine("[");
                Console.Write(message);
                _init = false;
            }
            else
            {
                Console.Write($",{Environment.NewLine}");
                Console.Write(message);
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
                Console.WriteLine();
                Console.WriteLine("]");
            }
            
            _isDisposed = true;
        }
    }
}
