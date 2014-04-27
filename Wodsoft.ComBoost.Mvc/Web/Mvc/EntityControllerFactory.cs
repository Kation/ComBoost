using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace System.Web.Mvc
{
    public class EntityControllerFactory : DefaultControllerFactory
    {
        private List<ControllerItem> _Items;

        public EntityControllerFactory()
        {
            _Items = new List<ControllerItem>();
        }

        public void RegisterController<TEntity>() where TEntity : class, IEntity, new()
        {
            Type type = typeof(TEntity);
            RegisterController(type);
        }

        public void RegisterController<TEntity>(string controller, string area = null) where TEntity : class, IEntity, new()
        {
            Type type = typeof(TEntity);
            if (controller == null)
                throw new ArgumentNullException("controller");
            RegisterController(type, controller, area);
        }

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

        public void UnregisterController<TEntity>() where TEntity : class, IEntity, new()
        {
            UnregisterController(typeof(TEntity));
        }

        public void UnregisterController(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            ControllerItem item = _Items.SingleOrDefault(t => t.EntityType == type);
            if (item != null)
                _Items.Remove(item);
        }

        protected override Type GetControllerType(Routing.RequestContext requestContext, string controllerName)
        {
            Type type = base.GetControllerType(requestContext, controllerName);
            if (type == null)
            {
                string areaString = requestContext.RouteData.DataTokens["Area"] as string;
                ControllerItem[] items;
                items = _Items.Where(t => t.Controller == controllerName.ToLower()).ToArray();
                if (items.Length > 1 && areaString != null)
                    items = items.Where(t => t.Area == areaString.ToLower()).ToArray();
                if (items.Length > 0)
                    type = typeof(EntityController<>).MakeGenericType(items[0].EntityType);
                if (type != null &&
                    ((areaString != null && items[0].Area == null) ||
                    (areaString != null && items[0].Area != areaString.ToLower()) ||
                    (areaString == null && items[0].Area != null)))
                    type = null;
            }
            return type;
        }

        private class ControllerItem
        {
            public string Area { get; set; }

            public string Controller { get; set; }

            public Type EntityType { get; set; }
        }
    }
}
