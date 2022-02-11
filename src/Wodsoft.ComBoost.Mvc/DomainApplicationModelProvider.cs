using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Mvc
{
    public class DomainApplicationModelProvider : IApplicationModelProvider
    {
        public int Order => 0;

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            //foreach (var controller in context.Result.Controllers)
            //{
            //    if (!typeof(IDomainController).IsAssignableFrom(controller.ControllerType))
            //        continue;
            //    foreach (var action in controller.Actions)
            //    {
            //        Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription
            //        action.ApiExplorer
            //    }
            //}
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {

        }
    }
}
