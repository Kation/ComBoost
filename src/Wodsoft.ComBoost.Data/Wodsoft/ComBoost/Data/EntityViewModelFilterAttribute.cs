using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data
{
    public class EntityViewModelFilterAttribute : DomainServiceFilterAttribute
    {
        public override async Task OnExecutionAsync(IDomainExecutionContext context, DomainExecutionPipeline next)
        {
            await next();
            if (context.Result is IViewModel viewModel)
            {
                await viewModel.UpdateItemsAsync();
            }
        }
    }
}
