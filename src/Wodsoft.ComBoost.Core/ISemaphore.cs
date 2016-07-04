using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface ISemaphore : IDisposable
    {
        Task EnterAsync();

        Task<bool> TryEnterAsync();

        Task<bool> TryEnterAsync(int timeout);

        Task<bool> EnterAsync(int timeout);

        Task ExitAsync();
    }
}
