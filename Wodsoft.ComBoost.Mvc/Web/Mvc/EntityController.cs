using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// Entity controller.
    /// </summary>
    public class EntityController : Controller
    {
        /// <summary>
        /// Get the context builder of entity.
        /// </summary>
        public IEntityContextBuilder EntityBuilder { get; private set; }

        /// <summary>
        /// Initialize entity controller.
        /// </summary>
        /// <param name="builder">Context builder of entity.</param>
        public EntityController(IEntityContextBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            EntityBuilder = builder;
        }

        /// <summary>
        /// Initialize entity controller.
        /// </summary>
        /// <typeparam name="T">Type of entity.</typeparam>
        /// <returns></returns>
        public EntityControllerUnitils<T> GetUnitils<T>()
            where T : class, IEntity, new()
        {
            return new EntityControllerUnitils<T>(this, EntityBuilder);
        }

        /// <summary>
        /// Called when authorization occurs.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
    }
}
