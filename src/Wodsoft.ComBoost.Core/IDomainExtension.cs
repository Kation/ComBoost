using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainExtension
    {
        Task OnDomainExecutingAsync(IDomainExecutionContext context);

        Task OnDomainExecutedAsync(IDomainExecutionContext context);
    }
}
