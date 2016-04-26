using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;

namespace System.Web.Mvc
{
    /// <summary>
    /// Entity extension class.
    /// </summary>
    public static class EntityExtension
    {
        static EntityExtension()
        {
            _EnumCache = new Dictionary<Type, EnumItem[]>();
        }

        /// <summary>
        /// Clear a query for a path.
        /// </summary>
        /// <param name="helper">A urlhelper.</param>
        /// <param name="name">Name of query.</param>
        /// <returns></returns>
        public static string ClearQueryPath(this UrlHelper helper, string name)
        {
            return ClearQueryPath(helper.RequestContext.HttpContext.Request.Url.PathAndQuery, name);
        }

        /// <summary>
        /// Clear a query for a path.
        /// </summary>
        /// <param name="helper">A urlhelper.</param>
        /// <param name="query">Query string.</param>
        /// <param name="name">Name of query.</param>
        /// <returns></returns>
        public static string ClearQueryPath(this UrlHelper helper, string query, string name)
        {
            return ClearQueryPath(query, name);
        }

        /// <summary>
        /// Set a query for a path.
        /// </summary>
        /// <param name="helper">A urlhelper.</param>
        /// <param name="query">Query string.</param>
        /// <param name="name">Name of query.</param>
        /// <returns></returns>
        public static string SetQueryPath(this UrlHelper helper, string query, string name)
        {
            return SetQueryPath(query, name);
        }

        /// <summary>
        /// Set a query for a path.
        /// </summary>
        /// <param name="helper">A urlhelper.</param>
        /// <param name="name">Name of query.</param>
        /// <returns></returns>
        public static string SetQueryPath(this UrlHelper helper, string name)
        {
            return SetQueryPath(helper.RequestContext.HttpContext.Request.Url.PathAndQuery, name);
        }

        private static string ClearQueryPath(string query, string name)
        {
            int index = query.IndexOf('?');
            if (index == -1)
                return query;
            index = query.IndexOf(name + "=", index);
            if (index == -1)
                return query;
            int end = query.IndexOf('&', index);
            if (end == -1)
                query = query.Substring(0, index);
            else
                query = query.Substring(0, index) + query.Substring(end + 1);
            if (query.EndsWith("&") || query.EndsWith("?"))
                query = query.Substring(0, query.Length - 1);
            return query;
        }

        private static string SetQueryPath(string query, string name)
        {
            query = ClearQueryPath(query, name);
            if (!query.EndsWith("&") || !query.EndsWith("?"))
            {
                if (query.Contains("?"))
                    query += "&";
                else
                    query += "?";
            }
            query += name + "=";
            return query;
        }

        /// <summary>
        /// Get a html string of pagination.
        /// </summary>
        /// <param name="helper">A html helper.</param>
        /// <param name="pagination">Entity view model.</param>
        public static MvcHtmlString Pagination(this HtmlHelper helper, IPagination pagination)
        {
            if (pagination == null)
                throw new ArgumentNullException("model");
            return helper.Partial("_Pagination", pagination);
        }

        /// <summary>
        /// Get a html string of pagination size button.
        /// </summary>
        /// <param name="helper">A html helper.</param>
        /// <param name="pagination">Entity view model.</param>
        public static MvcHtmlString PaginationButton(this HtmlHelper helper, IPagination pagination)
        {
            if (pagination == null)
                throw new ArgumentNullException("model");
            return helper.Partial("_PaginationButton", pagination);
        }

        /// <summary>
        /// Render a property editor.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <param name="helper">A html helper.</param>
        /// <param name="model">Entity model.</param>
        /// <param name="expression">Expression for property to entity.</param>
        /// <returns></returns>
        public static MvcHtmlString Editor<TEntity>(this HtmlHelper helper, IEntityEditModel<TEntity> model, Expression<Func<TEntity, object>> expression)
            where TEntity : class, IEntity, new()
        {
            if (!(expression.Body is MemberExpression))
                throw new NotSupportedException();
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            if (!(memberExpression.Expression is ParameterExpression))
                throw new NotSupportedException();
            var value = expression.Compile()(model.Item);
            var property = model.Metadata.GetProperty(memberExpression.Member.Name);
            return Editor(helper, model.Item, property, value);
        }

        /// <summary>
        /// Render a property editor.
        /// </summary>
        /// <param name="helper">A html helper.</param>
        /// <param name="entity">Entity object.</param>
        /// <param name="property">Property metadata.</param>
        /// <returns></returns>
        public static MvcHtmlString Editor(this HtmlHelper helper, IEntity entity, IPropertyMetadata property)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (property == null)
                throw new ArgumentNullException("property");
            return Editor(helper, entity, property, property.GetValue(entity));
        }

        /// <summary>
        /// Render a property editor.
        /// </summary>
        /// <param name="helper">A html helper.</param>
        /// <param name="entity">Entity object.</param>
        /// <param name="property">Property metadata.</param>
        /// <param name="value">Property value.</param>
        public static MvcHtmlString Editor(this HtmlHelper helper, IEntity entity, IPropertyMetadata property, object value)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (property == null)
                throw new ArgumentNullException("property");
            MvcEditorModel model = new MvcEditorModel();
            model.Metadata = property;
            model.Value = value;
            model.Entity = entity;
            if (property.Type == CustomDataType.Other)
                return helper.Partial(property.CustomType + "Editor", model);
            else
                return helper.Partial(property.Type.ToString() + "Editor", model);
        }
        
        /// <summary>
        /// Render a property viewer.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <param name="helper">A html helper.</param>
        /// <param name="model">Entity model.</param>
        /// <param name="expression">Expression for property to entity.</param>
        /// <returns></returns>
        public static MvcHtmlString Viewer<TEntity>(this HtmlHelper helper, IEntityEditModel<TEntity> model, Expression<Func<TEntity, object>> expression)
            where TEntity : class, IEntity, new()
        {
            if (!(expression.Body is MemberExpression))
                throw new NotSupportedException();
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            if (!(memberExpression.Expression is ParameterExpression))
                throw new NotSupportedException();
            var value = expression.Compile()(model.Item);
            var property = model.Metadata.GetProperty(memberExpression.Member.Name);
            return Viewer(helper, model.Item, property, value);
        }

        /// <summary>
        /// Render a property viewer.
        /// </summary>
        /// <param name="helper">A html helper.</param>
        /// <param name="entity">Entity object.</param>
        /// <param name="property">Property metadata.</param>
        /// <returns></returns>
        public static MvcHtmlString Viewer(this HtmlHelper helper, IEntity entity, IPropertyMetadata property)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (property == null)
                throw new ArgumentNullException("property");
            return Viewer(helper, entity, property, property.GetValue(entity));
        }

        /// <summary>
        /// Render a property viewer.
        /// </summary>
        /// <param name="helper">A html helper.</param>
        /// <param name="entity">Entity object.</param>
        /// <param name="property">Property metadata.</param>
        /// <param name="value">Property value.</param>
        public static MvcHtmlString Viewer(this HtmlHelper helper, IEntity entity, IPropertyMetadata property, object value)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (property == null)
                throw new ArgumentNullException("property");
            MvcEditorModel model = new MvcEditorModel();
            model.Metadata = property;
            model.Value = value;
            model.Entity = entity;
            if (property.Type == CustomDataType.Other)
                return helper.Partial(property.CustomType + "Viewer", model);
            else
                return helper.Partial(property.Type.ToString() + "Viewer", model);
        }

        /// <summary>
        /// Generates a fully qualified URL for the entity.
        /// </summary>
        /// <param name="helper">A urlhelper.</param>
        /// <param name="type">Type of entity.</param>
        /// <param name="action">Page name.</param>
        /// <returns></returns>
        public static string RouteUrl(this UrlHelper helper, Type type, string action)
        {
            MvcRouteAttribute route = type.GetCustomAttribute<MvcRouteAttribute>();
            if (route == null)
                return helper.RouteUrl(new { controller = type.Name, action = action });
            else
                if (route.Controller != null)
                    return helper.RouteUrl(new { area = route.Area, controller = route.Controller, action = action });
                else
                    return helper.RouteUrl(new { area = route.Area, controller = type.Name, action = action });
        }

        private static Dictionary<Type, EnumItem[]> _EnumCache;
        /// <summary>
        /// Analyze a enum type.
        /// </summary>
        /// <param name="helper">A html helper.</param>
        /// <param name="type">Type of enum.</param>
        /// <returns></returns>
        public static EnumItem[] EnumAnalyze(this HtmlHelper helper, Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException(type.Name + " is not a enum type.");
            if (!_EnumCache.ContainsKey(type))
            {
                var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                EnumItem[] list = new EnumItem[fields.Length];
                Type enumType = Enum.GetUnderlyingType(type);
                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    EnumItem item = new EnumItem();
                    DisplayAttribute display = field.GetCustomAttribute<DisplayAttribute>();
                    if (display == null)
                        item.Name = field.Name;
                    else
                        item.Name = display.Name;
                    item.Value = Convert.ChangeType(field.GetValue(null), enumType);
                    list[i] = item;
                }
                _EnumCache.Add(type, list);
            }
            return _EnumCache[type];
        }

        /// <summary>
        /// Get service provider from web view page.
        /// </summary>
        /// <param name="viewPage">Current view page.</param>
        /// <returns></returns>
        public static IServiceProvider GetServiceProvider(this WebViewPage viewPage)
        {
            Controller controller = viewPage.ViewContext.Controller as Controller;
            if (controller == null)
                return null;
            return GetServiceProvider(controller);
        }

        /// <summary>
        /// Get service provider from service provider.
        /// </summary>
        /// <param name="controller">Current controller.</param>
        /// <returns></returns>
        public static IServiceProvider GetServiceProvider(this Controller controller)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");
            object provider;
            if (!controller.ViewData.TryGetValue("DependencyServiceProvider", out provider))
            {
                provider = new DependencyServiceProvider(controller);
                controller.ViewData.Add("DependencyServiceProvider", provider);
            }
            return (DependencyServiceProvider)provider;
        }

        /// <summary>
        /// IServiceProvider wrapper of IDependencyResolver.
        /// </summary>
        public class DependencyServiceProvider : IServiceProvider
        {
            private Controller _Controller;

            /// <summary>
            /// Initialize dependency service provider.
            /// </summary>
            /// <param name="controller"></param>
            public DependencyServiceProvider(Controller controller)
            {
                if (controller == null)
                    throw new ArgumentNullException("controller");
                _Controller = controller;
                Resolver = controller.Resolver;
            }

            /// <summary>
            /// Get the dependency resolver.
            /// </summary>
            public IDependencyResolver Resolver { get; private set; }

            /// <summary>
            /// Get the service from dependency resolver.
            /// </summary>
            /// <param name="serviceType"></param>
            /// <returns></returns>
            public object GetService(Type serviceType)
            {
                if (typeof(Controller).IsAssignableFrom(serviceType))
                    return _Controller;
                else if (typeof(HttpRequestBase).IsAssignableFrom(serviceType))
                    return _Controller.Request;
                else if (typeof(HttpResponseBase).IsAssignableFrom(serviceType))
                    return _Controller.Response;
                else if (typeof(HttpServerUtilityBase).IsAssignableFrom(serviceType))
                    return _Controller.Server;
                return Resolver.GetService(serviceType);
            }
        }

        /// <summary>
        /// A data for enum item.
        /// </summary>
        public class EnumItem
        {
            /// <summary>
            /// Get or set the item name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Get or set the item value.
            /// </summary>
            public object Value { get; set; }
        }
    }
}