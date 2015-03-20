using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Web.Routing;

namespace System.Web.Mvc
{
    /// <summary>
    /// Entity controller factory.
    /// </summary>
    public class EntityControllerFactory : DefaultControllerFactory
    {
        private List<ControllerItem> _Items;

        /// <summary>
        /// Initialize entity controller factory.
        /// </summary>
        public EntityControllerFactory()
        {
            _Items = new List<ControllerItem>();
        }

        /// <summary>
        /// Register entity controller.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        public void RegisterController<TEntity>() where TEntity : class, IEntity, new()
        {
            Type type = typeof(TEntity);
            RegisterController(type);
        }

        /// <summary>
        /// Register entity controller.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <param name="controller">Controller name for entity.</param>
        /// <param name="area">Area name for entity.</param>
        public void RegisterController<TEntity>(string controller, string area = null) where TEntity : class, IEntity, new()
        {
            Type type = typeof(TEntity);
            if (controller == null)
                throw new ArgumentNullException("controller");
            RegisterController(type, controller, area);
        }

        /// <summary>
        /// Register entity controller.
        /// </summary>
        /// <param name="type">Type of entity.</param>
        public void RegisterController(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            MvcRouteAttribute route = type.GetCustomAttribute<MvcRouteAttribute>();
            if (route == null)
                RegisterController(type, type.Name, null);
            else
                RegisterController(type, route.Controller == null ? type.Name : route.Controller, route.Area);
        }

        /// <summary>
        /// Register entity controller.
        /// </summary>
        /// <param name="type">Type of entity.</param>
        /// <param name="controller">Controller name for entity.</param>
        /// <param name="area">Area name for entity.</param>
        public void RegisterController(Type type, string controller, string area = null)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (controller == null)
                throw new ArgumentNullException("controller");
            if (area != null)
            {
                if (_Items.Count(t => t.Area == area.ToLower() && t.Controller == controller.ToLower()) > 0)
                    throw new ArgumentException("Detected the same route of entity.");
            }
            else
            {
                if (_Items.Count(t => t.Controller == controller.ToLower()) > 0)
                    throw new ArgumentException("Detected the same route of entity.");

            }
            ControllerItem item = new ControllerItem();
            item.EntityType = type;
            item.Controller = controller.ToLower();
            if (area != null)
                item.Area = area.ToLower();
            _Items.Add(item);
        }

        /// <summary>
        /// Unregister entity controller.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        public void UnregisterController<TEntity>() where TEntity : class, IEntity, new()
        {
            UnregisterController(typeof(TEntity));
        }

        /// <summary>
        /// Unregister entity controller.
        /// </summary>
        /// <param name="type">Type of entity.</param>
        public void UnregisterController(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            ControllerItem item = _Items.SingleOrDefault(t => t.EntityType == type);
            if (item != null)
                _Items.Remove(item);
        }

        /// <summary>
        /// Retrieves the controller type for the specified name and request context.
        /// </summary>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The controller type.</returns>
        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            Type type = null;
            string areaString = requestContext.RouteData.DataTokens["Area"] as string;
            ControllerItem item;
            if (areaString == null)
                item = _Items.SingleOrDefault(t => t.Controller == controllerName.ToLower() && t.Area == null);
            else
                item = _Items.SingleOrDefault(t => t.Controller == controllerName.ToLower() && t.Area == areaString.ToLower());
            if (item != null)
                type = GetEntityControllerType(item.EntityType);
            if (type == null)
                type = base.GetControllerType(requestContext, controllerName);
            return type;
        }

        /// <summary>
        /// Get entity controller.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <returns>Entity Controller Type.</returns>
        protected virtual Type GetEntityControllerType(Type entityType)
        {
            return typeof(EntityController<>).MakeGenericType(entityType);
        }

        private class ControllerItem
        {
            public string Area { get; set; }

            public string Controller { get; set; }

            public Type EntityType { get; set; }
        }
    }
}
