using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Forum.Domain
{
    public class AliasFilter : DomainServiceFilterAttribute
    {
        public override Task OnExecutingAsync(IDomainExecutionContext context)
        {
            context.DomainContext.GetRequiredService<IConfigurableValueProvider>().SetAlias("id", "Index");
            return Task.CompletedTask;
        }
    }
}
