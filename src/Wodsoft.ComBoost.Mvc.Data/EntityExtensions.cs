using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Mvc
{
    public static class EntityExtensions
    {

        /// <summary>
        /// Render a property editor.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <param name="helper">A html helper.</param>
        /// <param name="model">Entity model.</param>
        /// <param name="expression">Expression for property to entity.</param>
        /// <returns></returns>
        public static IHtmlContent Editor<TEntity>(this IHtmlHelper helper, IEntityEditModel<TEntity> model, Expression<Func<TEntity, object>> expression)
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
        public static IHtmlContent Editor(this IHtmlHelper helper, IEntity entity, IPropertyMetadata property)
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
        /// Render a property viewer.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <param name="helper">A html helper.</param>
        /// <param name="model">Entity model.</param>
        /// <param name="expression">Expression for property to entity.</param>
        /// <returns></returns>
        public static IHtmlContent Viewer<TEntity>(this IHtmlHelper helper, IEntityEditModel<TEntity> model, Expression<Func<TEntity, object>> expression)
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
        /// Render a property viewer.
        /// </summary>
        /// <param name="helper">A html helper.</param>
        /// <param name="entity">Entity object.</param>
        /// <param name="property">Property metadata.</param>
        /// <param name="value">Property value.</param>
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
    }
}
