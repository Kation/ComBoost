using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface ISemaphoreProvider
    {
        ISemaphore GetMutex(string name);

        ISemaphore GetMonitor(string name);

        ISemaphore GetSemaphore(string name, int count);
    }
}
