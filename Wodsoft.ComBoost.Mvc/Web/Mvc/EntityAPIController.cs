using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public class EntityAPIController : Controller
    {
        /// <summary>
        /// Get the context builder of entity.
        /// </summary>
        public IEntityContextBuilder EntityBuilder { get; private set; }

        /// <summary>
        /// Initialize entity api controller.
        /// </summary>
        /// <param name="builder">Context builder of entity.</param>
        public EntityAPIController(IEntityContextBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            EntityBuilder = builder;
        }
    }
}
