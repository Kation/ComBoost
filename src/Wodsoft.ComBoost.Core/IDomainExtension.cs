using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainExtension
    {
        void OnInitialize(IServiceProvider serviceProvider, IDomainService domainService);

        Task OnExecutingAsync(IDomainExecutionContext context);

        Task OnExecutedAsync(IDomainExecutionContext context);
    }
}
