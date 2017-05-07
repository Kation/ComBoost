using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Wodsoft.ComBoost.Mvc
{
    public class ControllerDomainContext : MvcDomainContext
    {
        public ControllerDomainContext(Controller controller) : base(controller.ControllerContext)
        {
        }

        private MvcValueProvider _ValueProvider;
        protected override MvcValueProvider GetValueProvider()
        {
            if (_ValueProvider == null)
                _ValueProvider = new Mvc.MvcValueProvider(ActionContext);
            return _ValueProvider;
        }
    }
}
