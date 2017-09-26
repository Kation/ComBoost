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
    }
}
