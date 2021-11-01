using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public class MockServiceLifecycle : IMockServiceLifecycle
    {
        private List<Action> _disposeActions = new List<Action>();

        public void Register(Action disposeAction)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MockServiceLifecycle));
            _disposeActions.Add(disposeAction);
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                foreach (var item in _disposeActions)
                    item();
                _disposeActions.Clear();
                _disposeActions = null;
            }
        }
    }
}
