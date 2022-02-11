using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Mvc
{
    public class DomainApiDescriptionProvider : IApiDescriptionProvider
    {
        public int Order => 0;

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {

        }

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {

        }
    }
}
