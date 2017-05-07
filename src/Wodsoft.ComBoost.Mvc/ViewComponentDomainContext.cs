using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public class ViewComponentDomainContext : MvcDomainContext
    {
        public ViewComponentDomainContext(ViewComponent viewComponent)
            : base(viewComponent.ViewContext)
        {
            if (viewComponent == null)
                throw new ArgumentNullException(nameof(viewComponent));
            ViewComponent = viewComponent;
        }

        public ViewComponent ViewComponent { get; private set; }

        private MvcValueProvider _ValueProvider;
        protected override MvcValueProvider GetValueProvider()
        {
            if (_ValueProvider == null)
            {
                _ValueProvider = new MvcValueProvider(ActionContext);
                foreach (var arg in ViewComponent.ViewComponentContext.Arguments)
                    _ValueProvider.SetValue(arg.Key, arg.Value);
            }
            return _ValueProvider;
        }

        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IValueProvider))
                return ValueProvider;
            return base.GetService(serviceType);
        }
    }
}
