using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface ISemaphore
    {
        Task EnterAsync();

        Task<bool> TryEnterAsync();

        Task<bool> EnterAsync(int timeout);

        Task ExitAsync();
    }
}
