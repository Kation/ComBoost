using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityController<T> : EntityController
        where T : class, new()
    {
        public EntityController()
        { }
    }
}
