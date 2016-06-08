using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public class MvcDomainContext : DomainContext
    {
        public MvcDomainContext(Controller controller)
            : base(controller.HttpContext.RequestServices)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            Controller = controller;
        }

        public Controller Controller { get; private set; }

        public override object GetService(Type serviceType)
        {
            return base.GetService(serviceType);
        }
    }
}
