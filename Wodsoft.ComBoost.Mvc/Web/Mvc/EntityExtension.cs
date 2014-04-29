using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="helper">A htmlhelper.</param>
        /// <param name="model">Entity view model.</param>
        /// <returns></returns>
        public static MvcHtmlString Pagination(this HtmlHelper helper, EntityViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            string url = helper.ViewContext.Controller.ControllerContext.HttpContext.Request.Url.PathAndQuery;
            url = SetQueryPath(url, "page");

            string html = "<ul class=\"pagination\" style=\"margin-top: 0px; margin-bottom: 0px;\">\r\n";
            if (model.CurrentPage == 1)
                html += "    <li class=\"disabled\"><a>&laquo;</a></li>";
            else
                html += "    <li><a data-nav=\"true\" href=\"" + url + "1\">&laquo;</a></li>";

            int count = 0;
            while (count < 5 && model.CurrentPage - 2 + count <= model.TotalPage)
            {
                if (model.CurrentPage - 2 + count < 1)
                {
                    count++;
                    continue;
                }
                if (count == 2)
                    html += "    <li class=\"active\"><a>" + (model.CurrentPage - 2 + count) + "</a></li>";
                else
                    html += "    <li><a data-nav=\"true\" href=\"" + url + (model.CurrentPage - 2 + count) + "\">" + (model.CurrentPage - 2 + count) + "</a></li>";
                count++;
            }

            if (model.CurrentPage == model.TotalPage)
                html += "    <li class=\"disabled\"><a>&raquo;</a></li>";
            else
                html += "    <li><a data-nav=\"true\" href=\"" + url + model.TotalPage + "\">&raquo;</a></li>";
            html += "</ul>";
            return new MvcHtmlString(html);
        }

        /// <summary>
        /// Get a html string of pagination size button.
        /// </summary>
        /// <param name="helper">A htmlhelper.</param>
        /// <param name="model">Entity view model.</param>
        /// <returns></returns>
        public static MvcHtmlString PaginationButton(this HtmlHelper helper, EntityViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            string url = helper.ViewContext.Controller.ControllerContext.HttpContext.Request.Url.PathAndQuery;
            url = SetQueryPath(ClearQueryPath(url, "page"), "size");

            string html = "<div class=\"btn-group\">";
            foreach (var item in model.PageSizeOption)
            {
                html += " <a data-nav=\"true\" href=\"" + url + item + "\" class=\"btn btn-default" + (model.CurrentSize == item ? " active" : "") + "\">" + item + "</a>";
            }
            html += "</div>";
            return new MvcHtmlString(html);
        }

        /// <summary>
        /// Render a property editor.
        /// </summary>
        /// <param name="helper">A htmlhelper.</param>
        /// <param name="property">Property metadata.</param>
        /// <param name="value">Property value.</param>
        public static void Editor(this HtmlHelper helper, PropertyMetadata property, object value)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");
            if (property == null)
                throw new ArgumentNullException("property");
            MvcEditorModel model = new MvcEditorModel();
            model.Metadata = property;
            model.Value = value;
            if (property.Type == CustomDataType.Other)
                System.Web.Mvc.Html.RenderPartialExtensions.RenderPartial(helper, property.CustomType + "Editor", model);
            else
                System.Web.Mvc.Html.RenderPartialExtensions.RenderPartial(helper, property.Type.ToString() + "Editor", model);
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
        /// <param name="helper">A htmlhelper.</param>
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
                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    EnumItem item = new EnumItem();
                    DisplayAttribute display = field.GetCustomAttribute<DisplayAttribute>();
                    if (display == null)
                        item.Name = field.Name;
                    else
                        item.Name = display.Name;
                    item.Value = (int)field.GetValue(null);
                    list[i] = item;
                }
                _EnumCache.Add(type, list);
            }
            return _EnumCache[type];
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
            public int Value { get; set; }
        }
    }
}