using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public interface IMockServiceLifecycle : IDisposable
    {
        void Register(Action disposeAction);
    }
}
