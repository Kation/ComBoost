using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public class SkipFilterAttribute : DomainServiceFilterAttribute
    {
        public override Task OnExecutionAsync(IDomainExecutionContext context, DomainExecutionPipeline next)
        {
            return Task.CompletedTask;
        }
    }
}
