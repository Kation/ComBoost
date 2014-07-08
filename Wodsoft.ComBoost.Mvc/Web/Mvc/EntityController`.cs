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

namespace System.Web.Mvc
{
    /// <summary>
    /// Entity controller with actions.
    /// </summary>
    [EntityAuthorize]
    public class EntityController<TEntity> : EntityController where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// Metadata of entity.
        /// </summary>
        public EntityMetadata Metadata { get; private set; }

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
        public virtual async Task<ActionResult> Index(int page = 1, int size = 20, string parentpath = null, Guid? parentid = null, bool search = false)
        {
            if (page < 1)
                return new HttpStatusCodeResult(400);
            if (size < 1)
                return new HttpStatusCodeResult(400);
            if (!Metadata.ViewRoles.All(t => User.IsInRole(t)))
                return new HttpUnauthorizedResult();
            IQueryable<TEntity> queryable = EntityQueryable.Query();
            EntitySearchItem[] searchItems;
            if (search)
            {
                searchItems = GetSearchItem(ref queryable);
            }
            else
            {
                searchItems = new EntitySearchItem[0];
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
            var model = GetIndexModel(EntityQueryable.OrderBy(queryable), page, size);
            if (Metadata.ParentProperty != null && !search)
                model.Parent = GetParentModel(parentid, Metadata.ParentLevel);
            model.SearchItem = searchItems;
            model.Items = await EntityQueryable.ToArrayAsync(model.Queryable.Skip((model.CurrentPage - 1) * model.CurrentSize).Take(model.CurrentSize));
            return View(model);
        }

        protected virtual EntitySearchItem[] GetSearchItem(ref IQueryable<TEntity> queryable)
        {
            List<EntitySearchItem> searchItems = new List<EntitySearchItem>();
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
            return searchItems.ToArray();
        }

        protected virtual EntityViewModel<TEntity> GetIndexModel(IQueryable<TEntity> queryable, int page, int size)
        {
            var model = new EntityViewModel<TEntity>(EntityQueryable.OrderBy(queryable), page, size);
            //if (Metadata.ParentProperty != null && !search)
            //    model.Parent = GetParentModel(parentid, Metadata.ParentLevel);
            //model.SearchItem = searchItems;
            model.Headers = Metadata.ViewProperties;
            model.PageSizeOption = PageSize;
            //model.UpdateItems();
            return model;
        }

        protected virtual EntityParentModel[] GetParentModel(Guid? selected, int level)
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
                    if (entity == null)
                        continue;
                    EntityParentModel item = final.SingleOrDefault(t => t.Index == entity.Index);
                    if (item == null)
                    {
                        item = new EntityParentModel();
                        item.Path = path;
                        item.Name = entity.ToString();
                        item.Index = entity.Index;
                    }
                    if (selected.HasValue && item.Index == selected)
                        item.Selected = true;
                    //ParameterExpression dp = Expression.Parameter(metadata.Type);
                    //dynamic dChildren = _ESelectMethod.MakeGenericMethod(metadata.Type, typeof(Guid)).Invoke(null, new object[] { f, GetLambda(metadata.Type, typeof(Guid), Expression.Property(dp, typeof(EntityBase).GetProperty("BaseIndex")), dp) });
                    dynamic dChildren = _ESelectMethod.MakeGenericMethod(metadata.Type, typeof(Guid)).Invoke(null, new object[] { f, new Func<IEntity, Guid>(GetBaseIndex) });
                    Guid[] children = Linq.Enumerable.ToArray<Guid>(dChildren);
                    if (item.Items != null)
                        item.Items = item.Items.Concat(parents.Where(t => children.Contains(t.Index))).ToArray();
                    else
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

                level--;
                if (level == 0)
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
        public virtual async Task<ActionResult> Create(Guid? parent = null)
        {
            if (!EntityQueryable.Addable())
                return new HttpUnauthorizedResult();
            if (!Metadata.AddRoles.All(t => User.IsInRole(t)))
                return new HttpUnauthorizedResult();
            var model = await GetCreateModel(parent);
            return View("Edit", model);
        }

        protected virtual async Task<EntityEditModel<TEntity>> GetCreateModel(Guid? parent = null)
        {
            var model = new EntityEditModel<TEntity>(EntityQueryable.Create());
            model.Item.Index = Guid.Empty;
            model.Properties = Metadata.EditProperties;
            if (parent != null && model.Metadata.ParentProperty != null)
            {
                dynamic parentContext = EntityBuilder.GetContext(model.Metadata.ParentProperty.Property.PropertyType);
                object parentObj = await parentContext.GetEntityAsync(parent.Value);
                model.Metadata.ParentProperty.Property.SetValue(model.Item, parentObj);
            }
            return model;
        }

        /// <summary>
        /// Entity detail page.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<ActionResult> Detail(Guid id)
        {
            if (!Metadata.ViewRoles.All(t => User.IsInRole(t)))
                return new HttpUnauthorizedResult();
            var model = await GetDetailModel(id);
            if (model == null)
                return new HttpStatusCodeResult(404);
            return View(model);
        }

        protected virtual async Task<EntityEditModel<TEntity>> GetDetailModel(Guid id)
        {
            TEntity item = await EntityQueryable.GetEntityAsync(id);
            if (item == null)
                return null;
            var model = new EntityEditModel<TEntity>(item);
            model.Properties = Metadata.DetailProperties;
            return model;
        }

        /// <summary>
        /// Edit entity page.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<ActionResult> Edit(Guid id)
        {
            if (!EntityQueryable.Editable())
                return new HttpUnauthorizedResult();
            if (!User.Identity.IsAuthenticated && !Metadata.AllowAnonymous)
                return new HttpUnauthorizedResult();
            if (!Metadata.EditRoles.All(t => User.IsInRole(t)))
                return new HttpUnauthorizedResult();
            var model = await GetEditModel(id);
            if (model == null)
                return new HttpStatusCodeResult(404);
            return View(model);
        }

        protected virtual async Task<EntityEditModel<TEntity>> GetEditModel(Guid id)
        {
            TEntity item = await EntityQueryable.GetEntityAsync(id);
            if (item == null)
                return null;
            var model = new EntityEditModel<TEntity>(item);
            model.Properties = Metadata.EditProperties;
            return model;
        }

        /// <summary>
        /// Remove entity.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ActionResult> Remove(Guid id)
        {
            if (!EntityQueryable.Removeable())
                return new HttpUnauthorizedResult();
            if (!Metadata.RemoveRoles.All(t => User.IsInRole(t)))
                return new HttpUnauthorizedResult();
            if (await RemoveCore(id))
                return new HttpStatusCodeResult(200);
            else
                return new HttpStatusCodeResult(404);
        }

        protected virtual async Task<bool> RemoveCore(Guid id)
        {
            if (await EntityQueryable.RemoveAsync(id))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Update entity.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        [HttpPost]
        public virtual async Task<ActionResult> Update(Guid id)
        {
            TEntity entity;
            if (id == Guid.Empty)
            {
                if (!EntityQueryable.Addable())
                    return new HttpUnauthorizedResult();
                if (!Metadata.AddRoles.All(t => User.IsInRole(t)))
                    return new HttpUnauthorizedResult();
                entity = EntityQueryable.Create();
            }
            else
            {
                if (!EntityQueryable.Editable())
                    return new HttpUnauthorizedResult();
                if (!Metadata.EditRoles.All(t => User.IsInRole(t)))
                    return new HttpUnauthorizedResult();
                entity = EntityQueryable.GetEntity(id);
                if (entity == null)
                    return new HttpStatusCodeResult(404);
            }
            var properties = Metadata.Properties.Where(t => !t.IsHiddenOnEdit).ToArray();
            _ErrorMessage = null;
            if (!await UpdateCore(entity))
            {
                Response.StatusCode = 400;
                if (_ErrorMessage == null)
                    return Content("未知");
                else
                    return Content(_ErrorMessage);
            }
            return new HttpStatusCodeResult(200);
        }

        private string _ErrorMessage;
        protected virtual async Task<bool> UpdateCore(TEntity entity)
        {
            var properties = Metadata.Properties.Where(t => !t.IsHiddenOnEdit).ToArray();
            Parallel.ForEach(properties, property =>
            {
                UpdateProperty(entity, property);
            });
            ValidationContext validationContext = new ValidationContext(entity, new EntityDescriptorContext(EntityBuilder), null);
            var validateResult = entity.Validate(validationContext);
            if (validateResult.Count() != 0)
            {
                _ErrorMessage = string.Join("\r\n", validateResult.Select(t => t.ErrorMessage));
                return false;
            }
            bool result;
            if (entity.Index == Guid.Empty)
                result = await EntityQueryable.AddAsync(entity);
            else
                result = await EntityQueryable.EditAsync(entity);
            return result;
        }

        protected virtual void UpdateProperty(TEntity entity, PropertyMetadata propertyMetadata)
        {
            if (propertyMetadata.IsFileUpload)
            {
                #region File Path Value
                if (!Request.Files.AllKeys.Contains(propertyMetadata.Property.Name))
                    return;
                if (!(this is IFileController<TEntity>))
                    throw new NotSupportedException("Controller doesn't support upload file.");
                ((IFileController<TEntity>)this).SaveFileToProperty(entity, propertyMetadata, Request.Files[propertyMetadata.Property.Name]);
                #endregion
            }
            else
            {
                if (!Request.Form.AllKeys.Contains(propertyMetadata.Property.Name))
                    return;
                string value = Request.Form[propertyMetadata.Property.Name];
                TypeConverter converter = EntityValueConverter.GetConverter(propertyMetadata);
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
                    object v = propertyMetadata.Property.GetValue(entity);
                    if (v == null || value != v.ToString())
                        ((IPassword)entity).SetPassword(value);
                }
                else
                {
                    EntityValueConverterContext context = new EntityValueConverterContext(EntityBuilder.DescriptorContext, propertyMetadata);
                    object cvalue = converter.ConvertFrom(context, null, value);
                    if (converter.GetType() == typeof(Converter.CollectionConverter))
                    {
                        object collection = propertyMetadata.Property.GetValue(entity);
                        ((dynamic)collection).Clear();
                        var addMethod = collection.GetType().GetMethod("Add");
                        object[] array = (object[])cvalue;
                        for (int a = 0; a < array.Length; a++)
                            addMethod.Invoke(collection, new object[] { array[a] });
                    }
                    else
                    {
                        propertyMetadata.Property.SetValue(entity, cvalue);
                    }
                }
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
        public virtual async Task<ActionResult> Selector(int page = 1, string parentpath = null, Guid? parentid = null)
        {
            if (!User.Identity.IsAuthenticated && !Metadata.AllowAnonymous)
                return new HttpUnauthorizedResult();
            if (!Metadata.ViewRoles.All(t => User.IsInRole(t)))
                return new HttpUnauthorizedResult();
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
            var model = GetIndexModel(EntityQueryable.OrderBy(queryable), page, 10);
            if (Metadata.ParentProperty != null)
                model.Parent = GetParentModel(parentid, 3);
            model.Headers = Metadata.ViewProperties;
            model.Items = await EntityQueryable.ToArrayAsync(model.Queryable.Skip((model.CurrentPage - 1) * model.CurrentSize).Take(model.CurrentSize));
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
        public virtual async Task<ActionResult> MultipleSelector(int page = 1, string parentpath = null, Guid? parentid = null)
        {
            if (!User.Identity.IsAuthenticated && !Metadata.AllowAnonymous)
                return new HttpUnauthorizedResult();
            if (!Metadata.ViewRoles.All(t => User.IsInRole(t)))
                return new HttpUnauthorizedResult();
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
            var model = GetIndexModel(EntityQueryable.OrderBy(queryable), page, 10);
            if (Metadata.ParentProperty != null)
                model.Parent = GetParentModel(parentid, 3);
            model.Headers = Metadata.ViewProperties;
            model.Items = await EntityQueryable.ToArrayAsync(model.Queryable.Skip((model.CurrentPage - 1) * model.CurrentSize).Take(model.CurrentSize));
            return View(model);
        }

        /// <summary>
        /// Search page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Search()
        {
            if (!Metadata.ViewRoles.All(t => User.IsInRole(t)))
                return new HttpUnauthorizedResult();
            EntitySearchModel<TEntity> model = new EntitySearchModel<TEntity>();
            return View(model);
        }

        /// <summary>
        /// ValueFilterPage.
        /// </summary>
        /// <param name="property">Dependency property.</param>
        /// <param name="value">Value of dependency property.</param>
        /// <param name="selected">Value of selected.</param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult ValueFilter(string property, string value, string selected = null)
        {
            PropertyMetadata p = Metadata.GetProperty(property);
            if (p == null)
                return new HttpStatusCodeResult(404);
            ValueFilterAttribute filterAttribute = p.Property.GetCustomAttribute<ValueFilterAttribute>(true);
            if (filterAttribute == null)
                return new HttpStatusCodeResult(400);
            ValueFilter filter = (ValueFilter)Activator.CreateInstance(filterAttribute.ValueFilter);
            ViewBag.Selected = selected;
            ViewBag.IsRequired = p.IsRequired;
            var collection = filter.GetValues(filterAttribute.DependencyProperty, value);
            return View(collection);
        }
    }
}
