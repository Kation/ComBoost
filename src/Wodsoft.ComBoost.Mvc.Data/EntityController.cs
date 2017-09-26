using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityController : DomainController
    {
        public EntityController()
        {

        }

        private IDatabaseContext _DatabaseContext;
        protected IDatabaseContext DatabaseContext
        {
            get
            {
                if (_DatabaseContext == null)
                    _DatabaseContext = HttpContext.RequestServices.GetRequiredService<IDatabaseContext>();
                return _DatabaseContext;
            }
        }
    }
}
