using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public class HelloInterruptFilter : IDomainServiceFilter
    {
        public Task OnExecutionAsync(IDomainExecutionContext context, DomainExecutionPipeline next)
        {
            context.Done("Interrupt");
            return Task.CompletedTask;
        }
    }
}
