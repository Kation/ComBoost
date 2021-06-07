using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data
{
    public class EntityPagerFilter : IDomainServiceFilter
    {
        public async Task OnExecutionAsync(IDomainExecutionContext context, DomainExecutionPipeline next)
        {
            await next();
            if (context.Result is IViewModel viewModel)
            {
                var valueProvider = context.DomainContext.GetService<IValueProvider>();
                int page = valueProvider.GetValue<int>("page");
                int size = valueProvider.GetValue<int>("size");
                viewModel.SetPage(page);
                viewModel.SetSize(size);
                await viewModel.UpdateTotalPageAsync();
            }
        }
    }
}
