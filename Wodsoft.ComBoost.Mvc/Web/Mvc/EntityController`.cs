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
    /// Entity controller with actions.
    /// </summary>
    [EntityAuthorize]
    public class EntityController<TEntity> : EntityController where TEntity : class, IEntity, new()
    {
        private EntityMetadata Metadata;

        /// <summary>
        /// Get or set default page size for this controller.
        /// </summary>
        public int[] PageSize { get; protected set; }

        /// <summary>
        /// Get the queryable of entity.
        /// </summary>
        public IEntityQueryable<TEntity> EntityQueryable { get; private set; }

        /// <summary>
        /// Initialize entity controller.
        /// </summary>
        /// <param name="builder">Context builder of entity.</param>
        public EntityController(IEntityContextBuilder builder)
            : base(builder)
        {
            EntityQueryable = EntityBuilder.GetContext<TEntity>();
            Metadata = EntityAnalyzer.GetMetadata<TEntity>();
            PageSize = EntityViewModel.DefaultPageSizeOption;
        }

        /// <summary>
        /// Entity list page.
        /// </summary>
        /// <param name="page">Number of current page.</param>
        /// <param name="size">Number of entities per page.</param>
        /// <param name="parentpath">Path of parent for entity.</param>
        /// <param name="parentid">Parent id.</param>
        /// <param name="search">Is a search result.</param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Index(int page = 1, int size = 20, string parentpath = null, Guid? parentid = null, bool search = false)
        {
            if (page < 1)
                return new HttpStatusCodeResult(400);
            if (size < 1)
                return new HttpStatusCodeResult(400);
            if (!Metadata.ViewRoles.All(t => User.IsInRole(t)))
                return new HttpStatusCodeResult(403);
            IQueryable<TEntity> queryable = EntityQueryable.Query();
            List<EntitySearchItem> searchItems = new List<EntitySearchItem>();
            if (search)
            {
                var keys = Request.QueryString.AllKeys.Where(t => t.StartsWith("Search.")).Select(t => t.Substring(7).Split('.')).GroupBy(t => t[0], t => t.Length == 1 ? "" : "." + t[1]).ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    string propertyName = keys[i].Key;
                    PropertyMetadata property = Metadata.GetProperty(propertyName);
                    if (property == null || !property.Searchable)
                        continue;
                    EntitySearchItem searchItem = new EntitySearchItem();
                    string[] options = keys[i].ToArray();
                    switch (property.Type)
                    {
                        case ComponentModel.DataAnnotations.CustomDataType.Date:
                        case ComponentModel.DataAnnotations.CustomDataType.DateTime:
                            for (int a = 0; a < options.Length; a++)
                            {
                                if (options[a] == ".Start")
                                {
                                    DateTime start;
                                    if (!DateTime.TryParse(Request.QueryString["Search." + keys[i].Key + options[a]], out start))
                                        continue;
                                    searchItem.MorethanDate = start;
                                    ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                                    queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.GreaterThanOrEqual(Expression.Property(parameter, property.Property), Expression.Constant(start)), parameter));
                                }
                                else if (options[a] == ".End")
                                {
                                    DateTime end;
                                    if (!DateTime.TryParse(Request.QueryString["Search." + keys[i].Key + options[a]], out end))
                                        continue;
                                    searchItem.LessthanDate = end;
                                    ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                                    queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.LessThanOrEqual(Expression.Property(parameter, property.Property), Expression.Constant(end)), parameter));
                                }
                            }
                            break;
                        case ComponentModel.DataAnnotations.CustomDataType.Boolean:
                        case ComponentModel.DataAnnotations.CustomDataType.Sex:
                            if (options[0] == "")
                            {
                                bool result;
                                if (!bool.TryParse(Request.QueryString["Search." + keys[i].Key], out result))
                                    continue;
                                searchItem.Equal = result;
                                ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                                queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.Property(parameter, property.Property), Expression.Constant(result)), parameter));
                            }
                            break;
                        case ComponentModel.DataAnnotations.CustomDataType.Currency:
                        case ComponentModel.DataAnnotations.CustomDataType.Integer:
                        case ComponentModel.DataAnnotations.CustomDataType.Number:
                            for (int a = 0; a < options.Length; a++)
                            {
                                if (options[a] == ".Start")
                                {
                                    double start;
                                    if (!double.TryParse(Request.QueryString["Search." + keys[i].Key + options[a]], out start))
                                        continue;
                                    searchItem.Morethan = start;
                                    ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                                    queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.GreaterThanOrEqual(Expression.Property(parameter, property.Property), Expression.Constant(start)), parameter));
                                }
                                else if (options[a] == ".End")
                                {
                                    double end;
                                    if (!double.TryParse(Request.QueryString["Search." + keys[i].Key + options[a]], out end))
                                        continue;
                                    searchItem.Lessthan = end;
                                    ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                                    queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.LessThanOrEqual(Expression.Property(parameter, property.Property), Expression.Constant(end)), parameter));
                                }
                            }
                            break;
                        case ComponentModel.DataAnnotations.CustomDataType.Other:
                            if (property.CustomType == "Enum")
                            {
                                object result;
                                try
                                {
                                    result = Enum.Parse(property.Property.PropertyType, Request.QueryString["Search." + keys[i].Key]);
                                }
                                catch
                                {
                                    continue;
                                }
                                searchItem.Enum = new EnumConverter(property.Property.PropertyType).ConvertToString(result);
                                ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                                queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.Property(parameter, property.Property), Expression.Constant(result)), parameter));
                            }
                            else if (property.CustomType == "Entity")
                            {
                                searchItem.Contains = Request.QueryString["Search." + keys[i].Key];
                                ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                                Expression expression = Expression.Property(Expression.Property(parameter, property.Property), EntityAnalyzer.GetMetadata(property.Property.PropertyType).DisplayProperty.Property);
                                expression = Expression.Call(expression, typeof(string).GetMethod("Contains"), Expression.Constant(searchItem.Contains));
                                queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(expression, parameter));
                            }
                            break;
                        default:
                            if (property.Property.PropertyType == typeof(string))
                            {
                                searchItem.Contains = Request.QueryString["Search." + keys[i].Key];
                                ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                                Expression expression = Expression.Property(parameter, property.Property);
                                expression = Expression.Call(expression, typeof(string).GetMethod("Contains"), Expression.Constant(searchItem.Contains));
                                queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(expression, parameter));
                            }
                            break;
                    }
                    if (searchItem.Contains != null || searchItem.Enum != null || searchItem.Equal.HasValue || searchItem.Lessthan.HasValue || searchItem.LessthanDate.HasValue || searchItem.Morethan.HasValue || searchItem.MorethanDate.HasValue)
                        searchItem.Name = property.Name;
                    if (searchItem.Name != null)
                        searchItems.Add(searchItem);
                }
            }
            else
            {
                if (parentpath != null && parentid.HasValue)
                {
                    try
                    {
                        queryable = EntityQueryable.InParent(queryable, parentpath, parentid.Value);
                    }
                    catch
                    {
                        return new HttpStatusCodeResult(400);
                    }
                }
            }
            var model = new EntityViewModel<TEntity>(EntityQueryable.OrderBy(queryable), page, size);
            if (Metadata.ParentProperty != null && !search)
                model.Parent = GetParentModel(parentid);
            model.SearchItem = searchItems.ToArray();
            model.Headers = Metadata.ViewProperties;
            model.PageSizeOption = PageSize;
            model.UpdateItems();

            return View(model);
        }

        private EntityParentModel[] GetParentModel(Guid? selected)
        {
            EntityMetadata metadata = Metadata;

            List<EntityParentModel> final = new List<EntityParentModel>();
            ParameterExpression parameter = Expression.Parameter(Metadata.Type);
            //获取Parent属性组
            dynamic fl = GetGrouping(EntityQueryable.Query(), metadata.ParentProperty.Property.PropertyType, metadata.Type, GetLambda(metadata.Type, metadata.ParentProperty.Property.PropertyType, Expression.Property(parameter, Metadata.ParentProperty.Property), parameter));

            string path = metadata.ParentProperty.Property.Name;
            List<EntityParentModel> parents = null;
            while (parents == null || parents.Count > 0)
            {
                if (parents == null)
                    parents = new List<EntityParentModel>();
                List<EntityParentModel> temp = new List<EntityParentModel>();
                foreach (var f in fl)
                {
                    Type ft = f.GetType();
                    IEntity entity = ft.GetProperty("Key").GetValue(f);
                    EntityParentModel item = new EntityParentModel();
                    item.Path = path;
                    item.Name = entity.ToString();
                    item.Index = entity.Index;
                    if (selected.HasValue && item.Index == selected)
                        item.Selected = true;
                    //ParameterExpression dp = Expression.Parameter(metadata.Type);
                    //dynamic dChildren = _ESelectMethod.MakeGenericMethod(metadata.Type, typeof(Guid)).Invoke(null, new object[] { f, GetLambda(metadata.Type, typeof(Guid), Expression.Property(dp, typeof(EntityBase).GetProperty("BaseIndex")), dp) });
                    dynamic dChildren = _ESelectMethod.MakeGenericMethod(metadata.Type, typeof(Guid)).Invoke(null, new object[] { f, new Func<IEntity, Guid>(GetBaseIndex) });
                    Guid[] children = Linq.Enumerable.ToArray<Guid>(dChildren);
                    item.Items = parents.Where(t => children.Contains(t.Index)).ToArray();
                    if (!item.Selected && item.Items.Count(t => t.Selected) > 0)
                        item.Opened = true;
                    parents.RemoveAll(t => children.Contains(t.Index));
                    temp.Add(item);
                }
                final.AddRange(parents);
                parents = temp;

                Type groupType = typeof(IGrouping<,>).MakeGenericType(metadata.ParentProperty.Property.PropertyType, metadata.Type);

                metadata = EntityAnalyzer.GetMetadata(metadata.ParentProperty.Property.PropertyType);
                if (metadata.ParentProperty == null || parents.Count == 0)
                {
                    final.AddRange(parents);
                    break;
                }

                path += "." + metadata.ParentProperty.Property.Name;
                parameter = Expression.Parameter(groupType);
                var selectKey = GetLambda(groupType, metadata.Type, Expression.Property(parameter, groupType.GetProperty("Key")), parameter);
                parameter = Expression.Parameter(metadata.Type);
                fl = _QSelectMethod.MakeGenericMethod(groupType, metadata.Type).Invoke(null, new object[] { fl, selectKey });
                var groupByKey = GetLambda(metadata.Type, metadata.ParentProperty.Property.PropertyType, Expression.Property(parameter, metadata.ParentProperty.Property), parameter);
                fl = GetGrouping(fl, metadata.ParentProperty.Property.PropertyType, metadata.Type, groupByKey);
            }
            return final.ToArray();
        }

        private Guid GetBaseIndex(IEntity entity)
        {
            return entity.Index;
        }

        private static MethodInfo _ESelectMethod = typeof(Linq.Enumerable).GetMethods().Where(t => t.Name == "Select" && t.GetParameters().Length == 2).First();
        private static MethodInfo _QSelectMethod = typeof(Linq.Queryable).GetMethods().Where(t => t.Name == "Select" && t.GetParameters().Length == 2).First();
        private static MethodInfo _GroupByMethod = typeof(Linq.Queryable).GetMethods().Where(t => t.Name == "GroupBy" && t.GetParameters().Length == 2).First();
        private object GetGrouping(object queryable, Type tkey, Type tsource, Expression expression)
        {
            return _GroupByMethod.MakeGenericMethod(tsource, tkey).Invoke(null, new object[] { queryable, expression });
        }
        private Expression GetLambda(Type source, Type element, Expression expression, ParameterExpression parameter)
        {
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(source, element), expression, parameter);
        }

        /// <summary>
        /// Create entity page.
        /// </summary>
        /// <param name="parent">Parent id.</param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Create(Guid? parent = null)
        {
            if (!EntityQueryable.Addable())
                return new HttpStatusCodeResult(403);
            if (!Metadata.AddRoles.All(t => User.IsInRole(t)))
                return new HttpStatusCodeResult(403);
            var model = new EntityEditModel<TEntity>(EntityQueryable.Create());
            model.Item.Index = Guid.Empty;
            model.Properties = Metadata.EditProperties;
            if (parent != null && model.Metadata.ParentProperty != null)
            {
                dynamic parentContext = EntityBuilder.GetContext(model.Metadata.ParentProperty.Property.PropertyType);
                object parentObj = parentContext.GetEntity(parent.Value);
                model.Metadata.ParentProperty.Property.SetValue(model.Item, parentObj);
            }
            return View("Edit", model);
        }

        /// <summary>
        /// Entity detail page.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Detail(Guid id)
        {
            TEntity item = EntityQueryable.GetEntity(id);
            if (item == null)
                return new HttpStatusCodeResult(404);
            var model = new EntityEditModel<TEntity>(item);
            model.Properties = Metadata.DetailProperties;
            return View(model);
        }

        /// <summary>
        /// Edit entity page.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Edit(Guid id)
        {
            if (!EntityQueryable.Editable())
                return new HttpStatusCodeResult(403);
            if (!User.Identity.IsAuthenticated && !Metadata.AllowAnonymous)
                return new HttpStatusCodeResult(403);
            for (int i = 0; i < Metadata.EditRoles.Length; i++)
                if (!User.IsInRole(Metadata.EditRoles[i]))
                    return new HttpStatusCodeResult(403);
            TEntity item = EntityQueryable.GetEntity(id);
            if (item == null)
                return new HttpStatusCodeResult(404);
            var model = new EntityEditModel<TEntity>(item);
            model.Properties = Metadata.EditProperties;
            return View(model);
        }

        /// <summary>
        /// Remove entity.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult Remove(Guid id)
        {
            if (!EntityQueryable.Removeable())
                return new HttpStatusCodeResult(403);
            for (int i = 0; i < Metadata.RemoveRoles.Length; i++)
                if (!User.IsInRole(Metadata.RemoveRoles[i]))
                    return new HttpStatusCodeResult(403);
            if (EntityQueryable.Remove(id))
                return new HttpStatusCodeResult(200);
            else
                return new HttpStatusCodeResult(404);
        }

        /// <summary>
        /// Update entity.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        [HttpPost]
        public virtual ActionResult Update(Guid id)
        {
            TEntity entity;
            if (id == Guid.Empty)
            {
                if (!EntityQueryable.Addable())
                    return new HttpStatusCodeResult(403);
                for (int i = 0; i < Metadata.AddRoles.Length; i++)
                    if (!User.IsInRole(Metadata.AddRoles[i]))
                        return new HttpStatusCodeResult(403);
                entity = EntityQueryable.Create();
            }
            else
            {
                if (!EntityQueryable.Editable())
                    return new HttpStatusCodeResult(403);
                for (int i = 0; i < Metadata.EditRoles.Length; i++)
                    if (!User.IsInRole(Metadata.EditRoles[i]))
                        return new HttpStatusCodeResult(403);
                entity = EntityQueryable.GetEntity(id);
                if (entity == null)
                    return new HttpStatusCodeResult(404);
            }
            var properties = Metadata.Properties.Where(t => !t.IsHiddenOnEdit).ToArray();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyMetadata propertyMetadata = properties[i];
                if (propertyMetadata.Type == ComponentModel.DataAnnotations.CustomDataType.File || propertyMetadata.Type == ComponentModel.DataAnnotations.CustomDataType.Image)
                {
                    #region File Path Value
                    if (!Request.Files.AllKeys.Contains(propertyMetadata.Property.Name))
                        continue;
                    if (!(this is IFileController<TEntity>))
                        throw new NotSupportedException("Controller doesn't support upload file.");
                    ((IFileController<TEntity>)this).SaveFileToProperty(entity, propertyMetadata, Request.Files[propertyMetadata.Property.Name]);
                    #endregion
                }
                else
                {
                    #region Property Value
                    if (!Request.Form.AllKeys.Contains(propertyMetadata.Property.Name))
                    {
                        if (id == Guid.Empty && propertyMetadata.IsRequired)
                        {
                            Response.StatusCode = 400;
                            return Content(propertyMetadata.Name + "为必填项");
                        }
                        continue;
                    }
                    string originValue = Request.Form[propertyMetadata.Property.Name];
                    if (string.IsNullOrEmpty(originValue) && propertyMetadata.Property.PropertyType != typeof(string) && propertyMetadata.IsRequired)
                    {
                        Response.StatusCode = 400;
                        return Content(propertyMetadata.Name + "为必填项");
                    }
                    Type type = propertyMetadata.Property.PropertyType;
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        type = type.GetGenericArguments()[0];
                    TypeConverter converter = EntityValueConverter.GetConverter(type);
                    if (converter == null)
                        if (propertyMetadata.Property.PropertyType.IsGenericType && propertyMetadata.Property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                            converter = new Converter.CollectionConverter();
                        else if (propertyMetadata.Property.PropertyType.IsEnum)
                            converter = new EnumConverter(propertyMetadata.Property.PropertyType);
                        else
                            if (propertyMetadata.Type != ComponentModel.DataAnnotations.CustomDataType.Password)
                                throw new NotSupportedException("Type of \"" + propertyMetadata.Property.PropertyType.Name + "\" converter not found.");
                    if (propertyMetadata.Type == ComponentModel.DataAnnotations.CustomDataType.Password && entity is IPassword)
                    {
                        if (originValue != propertyMetadata.Property.GetValue(entity).ToString())
                            ((IPassword)entity).SetPassword(originValue);
                    }
                    else
                    {
                        EntityValueConverterContext context = new EntityValueConverterContext(EntityBuilder.DescriptorContext, propertyMetadata);
                        object value = converter.ConvertFrom(context, null, originValue);
                        if (converter.GetType() == typeof(Converter.CollectionConverter))
                        {
                            dynamic collection = propertyMetadata.Property.GetValue(entity);
                            collection.Clear();
                            object[] array = (object[])value;
                            for (int a = 0; a < array.Length; a++)
                                collection.Add(array[a]);
                        }
                        else
                        {
                            propertyMetadata.Property.SetValue(entity, value);
                        }
                    }
                    #endregion
                }
            }
            var validateResult = entity.Validate(null);
            if (validateResult.Count() != 0)
            {
                Response.StatusCode = 400;
                return Content(new string(validateResult.SelectMany(t => t.ErrorMessage += "/r/n").ToArray()));
            }
            bool result;
            if (id == Guid.Empty)
                result = EntityQueryable.Add(entity);
            else
                result = EntityQueryable.Edit(entity);
            if (result)
                return Content(entity.Index.ToString());
            else
            {
                Response.StatusCode = 400;
                return Content("未知");
            }
        }

        /// <summary>
        /// Selector page.
        /// </summary>
        /// <param name="page">Number of current page.</param>
        /// <param name="parentpath">Path of parent for entity.</param>
        /// <param name="parentid">Parent id.</param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Selector(int page = 1, string parentpath = null, Guid? parentid = null)
        {
            if (!User.Identity.IsAuthenticated && !Metadata.AllowAnonymous)
                return new HttpStatusCodeResult(403);
            for (int i = 0; i < Metadata.ViewRoles.Length; i++)
                if (!User.IsInRole(Metadata.ViewRoles[i]))
                    return new HttpStatusCodeResult(403);
            IQueryable<TEntity> queryable = EntityQueryable.Query();
            if (parentpath != null && parentid.HasValue)
            {
                try
                {
                    queryable = EntityQueryable.InParent(queryable, parentpath, parentid.Value);
                }
                catch
                {
                    return new HttpStatusCodeResult(400);
                }
            }
            var model = new EntityViewModel<TEntity>(EntityQueryable.OrderBy(queryable), page, 10);
            if (Metadata.ParentProperty != null)
                model.Parent = GetParentModel(parentid);
            model.Headers = Metadata.ViewProperties;
            model.UpdateItems();
            return View(model);
        }

        /// <summary>
        /// Multiple selector page.
        /// </summary>
        /// <param name="page">Number of current page.</param>
        /// <param name="parentpath">Path of parent for entity.</param>
        /// <param name="parentid">Parent id.</param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult MultipleSelector(int page = 1, string parentpath = null, Guid? parentid = null)
        {
            if (!User.Identity.IsAuthenticated && !Metadata.AllowAnonymous)
                return new HttpStatusCodeResult(403);
            for (int i = 0; i < Metadata.ViewRoles.Length; i++)
                if (!User.IsInRole(Metadata.ViewRoles[i]))
                    return new HttpStatusCodeResult(403);
            IQueryable<TEntity> queryable = EntityQueryable.Query();
            if (parentpath != null && parentid.HasValue)
            {
                try
                {
                    queryable = EntityQueryable.InParent(queryable, parentpath, parentid.Value);
                }
                catch
                {
                    return new HttpStatusCodeResult(400);
                }
            }
            var model = new EntityViewModel<TEntity>(EntityQueryable.OrderBy(queryable), page, 10);
            if (Metadata.ParentProperty != null)
                model.Parent = GetParentModel(parentid);
            model.Headers = Metadata.ViewProperties;
            model.UpdateItems();
            return View(model);
        }

        /// <summary>
        /// Search page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Search()
        {
            EntitySearchModel<TEntity> model = new EntitySearchModel<TEntity>();
            return View(model);
        }

    }
}
