using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public class MockDomainContextProvider : IDomainContextProvider, IDisposable
    {
        private MockDomainContext _context;
        private System.Threading.CancellationTokenSource _tokenSource;

        public MockDomainContextProvider(IServiceProvider serviceProvider)
        {
            _tokenSource = new System.Threading.CancellationTokenSource();
            _context = new MockDomainContext(serviceProvider, _tokenSource.Token);
        }

        private bool _disposed;
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _tokenSource.Cancel();
            }
        }

        public IDomainContext GetContext() => _context;
    }
}
