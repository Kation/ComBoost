using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public class DomainController : Controller
    {
        private List<IDomainProvider> _DomainProvider;

        public DomainController()
        {
            _DomainProvider = new List<IDomainProvider>();
        }

        #region DomainCollection Method

        protected void AddDomain(IDomainProvider serviceDescriptor)
        {
            if (serviceDescriptor == null)
                throw new ArgumentNullException(nameof(serviceDescriptor));
            _DomainProvider.Add(serviceDescriptor);
        }

        protected void ClearDomain()
        {
            _DomainProvider.Clear();
        }

        protected void RemoveDomain(IDomainProvider serviceDescriptor)
        {
            if (serviceDescriptor == null)
                throw new ArgumentNullException(nameof(serviceDescriptor));
            _DomainProvider.Remove(serviceDescriptor);
        }

        #endregion

        #region DomainService Method

        protected virtual Task ExecuteDomainServiceAsync(IDomainService domainService)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));
            return null;
        }

        protected virtual Task OnDomainServiceExecutingAsync(IDomainService domainService, DomainContext domainContext)
        {
            return null;
        }

        #endregion

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string actionName = context.RouteData.Values["action"] as string;
            IDomainService item = _DomainProvider.Select(t => t.GetService(actionName)).FirstOrDefault();
            if (item != null)
            {
                IDomainContext serviceContext = null;
                await item.Service(serviceContext);
                context.Result = OnOverrideServiceResult(item, serviceContext.Result);
            }
            await base.OnActionExecutionAsync(context, next);
        }

        protected virtual IActionResult OnOverrideServiceResult(IDomainService service, object result)
        {
            return View(result);
        }
    }
}
