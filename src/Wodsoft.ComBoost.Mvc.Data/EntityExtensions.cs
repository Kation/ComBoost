using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Mvc
{
    public static class EntityExtensions
    {

        /// <summary>
        /// 输出一个编辑器。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="helper">Html帮助器。</param>
        /// <param name="model">实体编辑模型。</param>
        /// <param name="expression">属性选择表达式。</param>
        /// <returns></returns>
        public static IHtmlContent Editor<TEntity>(this IHtmlHelper helper, IEditModel<TEntity> model, Expression<Func<TEntity, object>> expression)
            where TEntity : class, IEntity, new()
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (!(expression.Body is MemberExpression))
                throw new NotSupportedException();
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            if (!(memberExpression.Expression is ParameterExpression))
                throw new NotSupportedException();
            var value = expression.Compile()(model.Item);
            var metadata = EntityDescriptor.GetMetadata<TEntity>();
            var property = metadata.GetProperty(memberExpression.Member.Name);
            return Editor(helper, model.Item, property, value);
        }

        /// <summary>
        /// 输出一个编辑器。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TProperty">属性类型。</typeparam>
        /// <param name="helper">Html帮助器。</param>
        /// <param name="expression">属性选择表达式。</param>
        /// <returns></returns>
        public static IHtmlContent Editor<TEntity,TProperty>(this IHtmlHelper<IEditModel<TEntity>> helper, Expression<Func<TEntity,TProperty>> expression)
            where TEntity : IEntity
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (!(expression.Body is MemberExpression))
                throw new NotSupportedException();
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            if (!(memberExpression.Expression is ParameterExpression))
                throw new NotSupportedException();
            var value = expression.Compile()(helper.ViewData.Model.Item);
            var metadata = EntityDescriptor.GetMetadata<TEntity>();
            var property = metadata.GetProperty(memberExpression.Member.Name);
            return Editor(helper, helper.ViewData.Model.Item, property, value);
        }

        /// <summary>
        /// 输出一个编辑器。
        /// </summary>
        /// <param name="helper">Html帮助器。</param>
        /// <param name="entity">实体对象。</param>
        /// <param name="property">属性元数据。</param>
        /// <returns></returns>
        public static IHtmlContent Editor(this IHtmlHelper helper, IEntity entity, IPropertyMetadata property)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            if (helper == null)
                throw new ArgumentNullException("helper");
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (property == null)
                throw new ArgumentNullException("property");
            return Editor(helper, entity, property, property.GetValue(entity));
        }

        /// <summary>
        /// 输出一个编辑器。
        /// </summary>
        /// <param name="helper">Html帮助器。</param>
        /// <param name="entity">实体对象。</param>
        /// <param name="property">属性元数据。</param>
        /// <param name="value">属性值。</param>
        public static IHtmlContent Editor(this IHtmlHelper helper, IEntity entity, IPropertyMetadata property, object value)
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
        /// 输出一个查看器。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="helper">Html帮助器。</param>
        /// <param name="model">实体编辑模型。</param>
        /// <param name="expression">属性选择表达式。</param>
        /// <returns></returns>
        public static IHtmlContent Viewer<TEntity>(this IHtmlHelper helper, IEditModel<TEntity> model, Expression<Func<TEntity, object>> expression)
            where TEntity : class, IEntity, new()
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (!(expression.Body is MemberExpression))
                throw new NotSupportedException();
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            if (!(memberExpression.Expression is ParameterExpression))
                throw new NotSupportedException();
            var value = expression.Compile()(model.Item);
            var metadata = EntityDescriptor.GetMetadata<TEntity>();
            var property = metadata.GetProperty(memberExpression.Member.Name);
            return Viewer(helper, model.Item, property, value);
        }

        /// <summary>
        /// 输出一个查看器。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TProperty">属性类型。</typeparam>
        /// <param name="helper">Html帮助器。</param>
        /// <param name="expression">属性选择表达式。</param>
        /// <returns></returns>
        public static IHtmlContent Viewer<TEntity, TProperty>(this IHtmlHelper<IEditModel<TEntity>> helper, Expression<Func<TEntity, TProperty>> expression)
            where TEntity : IEntity
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (!(expression.Body is MemberExpression))
                throw new NotSupportedException();
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            if (!(memberExpression.Expression is ParameterExpression))
                throw new NotSupportedException();
            var value = expression.Compile()(helper.ViewData.Model.Item);
            var metadata = EntityDescriptor.GetMetadata<TEntity>();
            var property = metadata.GetProperty(memberExpression.Member.Name);
            return Viewer(helper, helper.ViewData.Model.Item, property, value);
        }

        /// <summary>
        /// 输出一个查看器。
        /// </summary>
        /// <param name="helper">Html帮助器。</param>
        /// <param name="entity">实体对象。</param>
        /// <param name="property">属性元数据。</param>
        /// <returns></returns>
        public static IHtmlContent Viewer(this IHtmlHelper helper, IEntity entity, IPropertyMetadata property)
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
        /// 输出一个查看器。
        /// </summary>
        /// <param name="helper">Html帮助器。</param>
        /// <param name="entity">实体对象。</param>
        /// <param name="property">属性元数据。</param>
        /// <param name="value">属性值。</param>
        public static IHtmlContent Viewer(this IHtmlHelper helper, IEntity entity, IPropertyMetadata property, object value)
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


        private static Dictionary<Type, EnumItem[]> _EnumCache = new Dictionary<Type, EnumItem[]>();
        /// <summary>
        /// Analyze a enum type.
        /// </summary>
        /// <param name="type">Type of enum.</param>
        /// <returns></returns>
        public static EnumItem[] EnumAnalyze(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!type.GetTypeInfo().IsEnum)
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
