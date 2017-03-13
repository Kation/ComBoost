using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Wodsoft.ComBoost.Mvc
{
    public class MvcDomainContext : DomainContext
    {
        public MvcDomainContext(Controller controller)
            : base(controller.HttpContext.RequestServices, controller.HttpContext.RequestAborted)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            Controller = controller;
            _Options = new DomainServiceOptions();
        }
        
        public Controller Controller { get; private set; }

        private MvcValueProvider _ValueProvider;
        public MvcValueProvider ValueProvider
        {
            get
            {
                if (_ValueProvider == null)
                    _ValueProvider = new MvcValueProvider(Controller);
                return _ValueProvider;
            }
        }

        private IDomainServiceOptions _Options;
        public override IDomainServiceOptions Options
        {
            get
            {
                return _Options;
            }
        }

        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IValueProvider))
                return ValueProvider;
            return base.GetService(serviceType);
        }
    }
}
