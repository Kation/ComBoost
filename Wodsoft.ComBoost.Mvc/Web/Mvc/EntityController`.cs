using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Web.Mvc
{
    /// <summary>
    /// Entity controller with actions.
    /// </summary>
    [EntityAuthorize]
    public class EntityController<TEntity> : EntityController, IHaveEntityMetadata where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// Metadata of entity.
        /// </summary>
        public IEntityMetadata Metadata { get; private set; }

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
            PageSize = Pagination.DefaultPageSizeOption;
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
        [EntityAuthorize(EntityAuthorizeAction.View)]
        public virtual async Task<ActionResult> Index(int page = 1, int size = 20, string parentpath = null, Guid? parentid = null, bool search = false)
        {
            if (page < 1)
                return new HttpStatusCodeResult(400);
            if (size < 1)
                return new HttpStatusCodeResult(400);
            //if (!Metadata.ViewRoles.All(t => User.IsInRole(t)))
            //    return new HttpUnauthorizedResult();
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
            var model = await GetIndexModel(EntityQueryable.OrderBy(queryable), page, size);
            if (Metadata.ParentProperty != null && !search)
                model.Parent = await GetParentModel(parentid);
            model.SearchItem = searchItems;
            model.Items = await EntityQueryable.ToArrayAsync(model.Queryable.Skip((model.CurrentPage - 1) * model.CurrentSize).Take(model.CurrentSize));
            if (model.ViewButtons.Length > 0)
            {
                IServiceProvider serviceProvider = this.GetServiceProvider();
                foreach (var item in model.ViewButtons)
                    item.SetTarget(serviceProvider);
            }
            return View(model);
        }

        /// <summary>
        /// Get search item in request.
        /// </summary>
        /// <param name="queryable">Queryable of TEntity.</param>
        /// <returns>Search item of TEntity.</returns>
        protected virtual EntitySearchItem[] GetSearchItem(ref IQueryable<TEntity> queryable)
        {
            List<EntitySearchItem> searchItems = new List<EntitySearchItem>();
            var keys = Request.QueryString.AllKeys.Where(t => t.StartsWith("Search.")).Select(t => t.Substring(7).Split('.')).GroupBy(t => t[0], t => t.Length == 1 ? "" : "." + t[1]).ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                string propertyName = keys[i].Key;
                IPropertyMetadata property = Metadata.GetProperty(propertyName);
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
                                queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.GreaterThanOrEqual(Expression.Property(parameter, property.ClrName), Expression.Constant(start)), parameter));
                            }
                            else if (options[a] == ".End")
                            {
                                DateTime end;
                                if (!DateTime.TryParse(Request.QueryString["Search." + keys[i].Key + options[a]], out end))
                                    continue;
                                searchItem.LessthanDate = end;
                                ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                                queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.LessThanOrEqual(Expression.Property(parameter, property.ClrName), Expression.Constant(end)), parameter));
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
                            queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.Property(parameter, property.ClrName), Expression.Constant(result)), parameter));
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
                                queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.GreaterThanOrEqual(Expression.Property(parameter, property.ClrName), Expression.Constant(start)), parameter));
                            }
                            else if (options[a] == ".End")
                            {
                                double end;
                                if (!double.TryParse(Request.QueryString["Search." + keys[i].Key + options[a]], out end))
                                    continue;
                                searchItem.Lessthan = end;
                                ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                                queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.LessThanOrEqual(Expression.Property(parameter, property.ClrName), Expression.Constant(end)), parameter));
                            }
                        }
                        break;
                    case ComponentModel.DataAnnotations.CustomDataType.Other:
                        if (property.CustomType == "Enum")
                        {
                            object result;
                            try
                            {
                                result = Enum.Parse(property.ClrType, Request.QueryString["Search." + keys[i].Key]);
                            }
                            catch
                            {
                                continue;
                            }
                            searchItem.Enum = new EnumConverter(property.ClrType).ConvertToString(result);
                            ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                            queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.Property(parameter, property.ClrName), Expression.Constant(result)), parameter));
                        }
                        else if (property.CustomType == "Entity")
                        {
                            searchItem.Contains = Request.QueryString["Search." + keys[i].Key];
                            ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                            Expression expression = Expression.Property(Expression.Property(parameter, property.ClrName), EntityAnalyzer.GetMetadata(property.ClrType).DisplayProperty.ClrName);
                            expression = Expression.Call(expression, typeof(string).GetMethod("Contains"), Expression.Constant(searchItem.Contains));
                            queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(expression, parameter));
                        }
                        break;
                    default:
                        if (property.ClrType == typeof(string))
                        {
                            searchItem.Contains = Request.QueryString["Search." + keys[i].Key];
                            ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                            Expression expression = Expression.Property(parameter, property.ClrName);
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

        /// <summary>
        /// Get index action model.
        /// </summary>
        /// <param name="queryable">Queryable object of TEntity.</param>
        /// <param name="page">Current page.</param>
        /// <param name="size">Current page size.</param>
        /// <returns>View model of TEntity.</returns>
        protected virtual async Task<EntityViewModel<TEntity>> GetIndexModel(IQueryable<TEntity> queryable, int page, int size)
        {
            return await Task.Run<EntityViewModel<TEntity>>(() =>
            {
                var model = new EntityViewModel<TEntity>(EntityQueryable.OrderBy(queryable), page, size);
                //if (Metadata.ParentProperty != null && !search)
                //    model.Parent = GetParentModel(parentid, Metadata.ParentLevel);
                //model.SearchItem = searchItems;
                model.Headers = Metadata.ViewProperties.Where(t => (t.AllowAnonymous || User.Identity.IsAuthenticated) && !t.ViewRoles.Any(r => !User.IsInRole(r))).ToArray();
                model.PageSizeOption = PageSize;
                //model.UpdateItems();
                return model;
            });
        }

        /// <summary>
        /// Get parent model.
        /// </summary>
        /// <param name="selected">Selected parent id.</param>
        /// <param name="level">Max tree level.</param>
        /// <returns>Parent model.</returns>
        protected virtual async Task<EntityParentModel[]> GetParentModel(Guid? selected)
        {
            List<EntityParentModel> root = new List<EntityParentModel>();
            await GetParentModel(root, new Dictionary<Guid, EntityParentModel[]>(), Metadata, selected, Metadata.ParentProperty.ClrName);
            return root.ToArray();
        }

        private async Task GetParentModel(List<EntityParentModel> root, Dictionary<Guid, EntityParentModel[]> items, IEntityMetadata metadata, Guid? selected, string path)
        {
            IEntityMetadata parentMetadata = EntityAnalyzer.GetMetadata(metadata.ParentProperty.ClrType);

            dynamic context = EntityBuilder.GetContext(parentMetadata.Type);
            dynamic queryable = context.OrderBy();

            if (parentMetadata.ParentProperty == null)
            {
                IEntity[] result = await context.ToArrayAsync(queryable);
                foreach (var item in result)
                {
                    EntityParentModel model = new EntityParentModel();
                    model.Index = item.Index;
                    model.Name = item.ToString();
                    model.Path = path;
                    model.IsSelected = selected == item.Index;
                    if (items.ContainsKey(model.Index))
                        model.Items = items[model.Index];
                    else
                        model.Items = new EntityParentModel[0];
                    root.Add(model);
                }
            }
            else
            {
                Dictionary<Guid, EntityParentModel[]> newItems = new Dictionary<Guid, EntityParentModel[]>();

                var parameter = Expression.Parameter(parentMetadata.Type);
                var parent = Expression.Property(parameter, parentMetadata.ParentProperty.ClrName);
                var expression = Expression.Lambda(parent, parameter);
                dynamic result = GetGrouping(queryable, parentMetadata.ParentProperty.ClrType, parentMetadata.Type, expression);

                List<EntityParentModel> thisRoot = null;
                if (parentMetadata.ParentProperty.ClrType == parentMetadata.Type)
                    thisRoot = new List<EntityParentModel>();
                Type groupType = typeof(IGrouping<,>).MakeGenericType(parentMetadata.ParentProperty.ClrType, parentMetadata.Type);
                foreach (dynamic item in result)
                {
                    List<EntityParentModel> models = new List<EntityParentModel>();
                    IEntity key = groupType.GetProperty("Key").GetValue(item) as IEntity;
                    foreach (IEntity entity in item)
                    {
                        EntityParentModel model = new EntityParentModel();
                        model.Index = entity.Index;
                        model.Name = entity.ToString();
                        model.Path = path;
                        model.IsSelected = selected == entity.Index;
                        if (items.ContainsKey(model.Index))
                            model.Items = items[model.Index];
                        else
                            model.Items = new EntityParentModel[0];
                        models.Add(model);
                    }
                    if (key == null)
                    {
                        root.AddRange(models);
                        if (parentMetadata.ParentProperty.ClrType == parentMetadata.Type)
                            thisRoot.AddRange(models);
                    }
                    else
                        newItems.Add(key.Index, models.ToArray());
                }
                if (parentMetadata.ParentProperty.ClrType == parentMetadata.Type)
                {
                    ExpendTree(thisRoot, newItems);
                }
                else
                {
                    await GetParentModel(root, newItems, parentMetadata, selected, path + "." + parentMetadata.ParentProperty.ClrName);
                }
            }
        }

        private void ExpendTree(List<EntityParentModel> root, Dictionary<Guid, EntityParentModel[]> items)
        {
            while (root.Count > 0)
            {
                List<EntityParentModel> newRoot = new List<EntityParentModel>();
                foreach (var item in root)
                {
                    if (items.ContainsKey(item.Index))
                    {
                        item.Items = item.Items.Concat(items[item.Index]).ToArray();
                        newRoot.AddRange(items[item.Index]);
                        items.Remove(item.Index);
                    }
                }
                root = newRoot;
            }
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
        [EntityAuthorize(EntityAuthorizeAction.Create)]
        public virtual async Task<ActionResult> Create(Guid? parent = null)
        {
            if (!EntityQueryable.Addable())
                return new HttpStatusCodeResult(403);
            //if (!Metadata.AddRoles.All(t => User.IsInRole(t)))
            //    return new HttpUnauthorizedResult();
            var model = await GetCreateModel(parent);
            return View("Edit", model);
        }

        /// <summary>
        /// Get create action model.
        /// </summary>
        /// <param name="parent">Parent id.</param>
        /// <returns>Edit model with new entity item of TEntity.</returns>
        protected virtual async Task<EntityEditModel<TEntity>> GetCreateModel(Guid? parent = null)
        {
            var model = new EntityEditModel<TEntity>(EntityQueryable.Create());
            model.Item.Index = Guid.Empty;
            model.Properties = Metadata.CreateProperties.Where(t => (t.AllowAnonymous || User.Identity.IsAuthenticated) && !t.EditRoles.Any(r => !User.IsInRole(r))).ToArray();
            if (parent != null && model.Metadata.ParentProperty != null)
            {
                dynamic parentContext = EntityBuilder.GetContext(model.Metadata.ParentProperty.ClrType);
                object parentObj = await parentContext.GetEntityAsync(parent.Value);
                model.Metadata.ParentProperty.SetValue(model.Item, parentObj);
            }
            return model;
        }

        /// <summary>
        /// Entity detail page.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [HttpGet]
        [EntityAuthorize(EntityAuthorizeAction.View)]
        public virtual async Task<ActionResult> Detail(Guid id)
        {
            //if (!Metadata.ViewRoles.All(t => User.IsInRole(t)))
            //    return new HttpUnauthorizedResult();
            var model = await GetDetailModel(id);
            if (model == null)
                return new HttpStatusCodeResult(404);
            return View(model);
        }

        /// <summary>
        /// Get detail action model.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Edit model with detail properties of TEntity.</returns>
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
        [EntityAuthorize(EntityAuthorizeAction.Edit)]
        public virtual async Task<ActionResult> Edit(Guid id)
        {
            if (!EntityQueryable.Editable())
                return new HttpStatusCodeResult(403);
            //if (!User.Identity.IsAuthenticated && !Metadata.AllowAnonymous)
            //    return new HttpUnauthorizedResult();
            //if (!Metadata.EditRoles.All(t => User.IsInRole(t)))
            //    return new HttpUnauthorizedResult();
            var model = await GetEditModel(id);
            if (model == null)
                return new HttpStatusCodeResult(404);
            return View(model);
        }

        /// <summary>
        /// Get edit action model.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Edit model of TEntity.</returns>
        protected virtual async Task<EntityEditModel<TEntity>> GetEditModel(Guid id)
        {
            TEntity item = await EntityQueryable.GetEntityAsync(id);
            if (item == null)
                return null;
            var model = new EntityEditModel<TEntity>(item);
            model.Properties = Metadata.EditProperties.Where(t => (t.AllowAnonymous || User.Identity.IsAuthenticated) && !t.EditRoles.Any(r => !User.IsInRole(r))).ToArray();
            return model;
        }

        /// <summary>
        /// Remove entity.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [HttpPost]
        [EntityAuthorize(EntityAuthorizeAction.Remove)]
        public virtual async Task<ActionResult> Remove(Guid id)
        {
            if (!EntityQueryable.Removeable())
                return new HttpStatusCodeResult(403);
            if (await RemoveCore(id))
                return new HttpStatusCodeResult(200);
            else
                return new HttpStatusCodeResult(404);
        }

        /// <summary>
        /// Remoce action core method.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>True if success, otherwise is false.</returns>
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
        [EntityAuthorize]
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
            ErrorMessage = null;
            if (!await UpdateCore(entity))
            {
                Response.StatusCode = 400;
                //Important!!!
                Response.TrySkipIisCustomErrors = true;
                if (ErrorMessage == null)
                    return Content("未知");
                else
                    return Content(ErrorMessage);
            }
            return Content(entity.Index.ToString());
        }

        /// <summary>
        /// Get or set the error message.
        /// This will make update action stop and return error message.
        /// </summary>
        protected string ErrorMessage { get; set; }

        /// <summary>
        /// Update action core method.
        /// </summary>
        /// <param name="entity">Entity to edit or create.</param>
        /// <returns>True if success, otherwise is false.</returns>
        protected virtual async Task<bool> UpdateCore(TEntity entity)
        {
            var properties = Metadata.EditProperties.ToArray();
            foreach (var property in properties)
            {
                await UpdateProperty(entity, property);
            }
            if (ErrorMessage != null)
                return false;
            ValidationContext validationContext = new ValidationContext(entity, new EntityDescriptorContext(EntityBuilder), null);
            var validateResult = entity.Validate(validationContext);
            if (validateResult.Count() != 0)
            {
                ErrorMessage = string.Join("\r\n", validateResult.Select(t => t.ErrorMessage));
                return false;
            }
            bool result;
            if (entity.Index == Guid.Empty)
                result = await EntityQueryable.AddAsync(entity);
            else
                result = await EntityQueryable.EditAsync(entity);
            return result;
        }

        /// <summary>
        /// Update property for entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <param name="propertyMetadata">Property metadata.</param>
        /// <returns></returns>
        protected virtual async Task UpdateProperty(TEntity entity, IPropertyMetadata propertyMetadata)
        {
            if (propertyMetadata.EditRoles.Any(t => !User.IsInRole(t)))
                return;
            if (propertyMetadata.IsFileUpload)
            {
                #region File Path Value
                if (!Request.Files.AllKeys.Contains(propertyMetadata.ClrName))
                    return;
                if (!(this is IFileController<TEntity>))
                {
                    if (propertyMetadata.ClrType != typeof(byte[]))
                        throw new NotSupportedException("Controller doesn't support upload file.");
                    if (Request.Files.AllKeys.Contains(propertyMetadata.ClrName))
                    {
                        var file = Request.Files[propertyMetadata.ClrName];
                        MemoryStream stream = new MemoryStream();
                        await file.InputStream.CopyToAsync(stream);
                        propertyMetadata.SetValue(entity, stream.ToArray());
                        stream.Close();
                        stream.Dispose();
                    }
                }
                else
                    await ((IFileController<TEntity>)this).SaveFileToProperty(entity, propertyMetadata, Request.Files[propertyMetadata.ClrName]);
                #endregion
            }
            else
            {
                if (!Request.Form.AllKeys.Contains(propertyMetadata.ClrName))
                    return;
                string value = Request.Form[propertyMetadata.ClrName];
                TypeConverter converter = EntityValueConverter.GetConverter(propertyMetadata);
                if (converter == null)
                    if (propertyMetadata.ClrType.IsGenericType && propertyMetadata.ClrType.GetGenericTypeDefinition() == typeof(ICollection<>))
                        converter = new Converter.CollectionConverter();
                    else if (propertyMetadata.ClrType.IsEnum)
                        converter = new EnumConverter(propertyMetadata.ClrType);
                    else
                    {
                        converter = TypeDescriptor.GetConverter(propertyMetadata.ClrType);
                        if (converter == null && propertyMetadata.Type != ComponentModel.DataAnnotations.CustomDataType.Password)
                            throw new NotSupportedException("Type of \"" + propertyMetadata.ClrName + "\" converter not found.");
                    }
                if (propertyMetadata.Type == ComponentModel.DataAnnotations.CustomDataType.Password && entity is IPassword)
                {
                    object v = propertyMetadata.GetValue(entity);
                    if (v == null || value != v.ToString())
                        ((IPassword)entity).SetPassword(value);
                }
                else
                {
                    EntityValueConverterContext context = new EntityValueConverterContext(EntityBuilder.DescriptorContext, propertyMetadata);
                    object cvalue;
                    try
                    {
                        cvalue = converter.ConvertFrom(context, null, value);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = ex.Message;
                        return;
                    }
                    if (converter.GetType() == typeof(Converter.CollectionConverter))
                    {
                        object collection = propertyMetadata.GetValue(entity);
                        ((dynamic)collection).Clear();
                        var addMethod = collection.GetType().GetMethod("Add");
                        object[] array = (object[])cvalue;
                        for (int a = 0; a < array.Length; a++)
                            addMethod.Invoke(collection, new object[] { array[a] });
                    }
                    else
                    {
                        propertyMetadata.SetValue(entity, cvalue);
                    }
                }
            }
        }

        /// <summary>
        /// Selector page.
        /// </summary>
        /// <param name="page">Number of current page.</param>
        /// <param name="size">Number of entities per page.</param>
        /// <param name="parentpath">Path of parent for entity.</param>
        /// <param name="parentid">Parent id.</param>
        /// <param name="search">Is a search result.</param>
        /// <returns></returns>
        [HttpGet]
        [EntityAuthorize(EntityAuthorizeAction.View)]
        public virtual async Task<ActionResult> Selector(int page = 1, int size = 10, string parentpath = null, Guid? parentid = null, bool search = false)
        {
            if (!User.Identity.IsAuthenticated && !Metadata.AllowAnonymous)
                return new HttpUnauthorizedResult();
            //if (!Metadata.ViewRoles.All(t => User.IsInRole(t)))
            //    return new HttpUnauthorizedResult();
            IQueryable<TEntity> queryable = EntityQueryable.Query();
            EntitySearchItem[] searchItems = null;
            if (search)
            {
                searchItems = GetSearchItem(ref queryable);
            }
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
            var model = await GetIndexModel(EntityQueryable.OrderBy(queryable), page, size);
            if (Metadata.ParentProperty != null)
                model.Parent = await GetParentModel(parentid);
            model.Headers = Metadata.ViewProperties;
            model.SearchItem = searchItems;
            model.Items = await EntityQueryable.ToArrayAsync(model.Queryable.Skip((model.CurrentPage - 1) * model.CurrentSize).Take(model.CurrentSize));
            return View(model);
        }

        /// <summary>
        /// Multiple selector page.
        /// </summary>
        /// <param name="page">Number of current page.</param>
        /// <param name="size">Number of entities per page.</param>
        /// <param name="parentpath">Path of parent for entity.</param>
        /// <param name="parentid">Parent id.</param>
        /// <param name="search">Is a search result.</param>
        /// <returns></returns>
        [HttpGet]
        [EntityAuthorize(EntityAuthorizeAction.View)]
        public virtual async Task<ActionResult> MultipleSelector(int page = 1, int size = 10, string parentpath = null, Guid? parentid = null, bool search = false)
        {
            if (!User.Identity.IsAuthenticated && !Metadata.AllowAnonymous)
                return new HttpUnauthorizedResult();
            //if (!Metadata.ViewRoles.All(t => User.IsInRole(t)))
            //    return new HttpUnauthorizedResult();
            IQueryable<TEntity> queryable = EntityQueryable.Query();
            EntitySearchItem[] searchItems = null;
            if (search)
            {
                searchItems = GetSearchItem(ref queryable);
            }
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
            var model = await GetIndexModel(EntityQueryable.OrderBy(queryable), page, size);
            if (Metadata.ParentProperty != null)
                model.Parent = await GetParentModel(parentid);
            model.Headers = Metadata.ViewProperties;
            model.SearchItem = searchItems;
            model.Items = await EntityQueryable.ToArrayAsync(model.Queryable.Skip((model.CurrentPage - 1) * model.CurrentSize).Take(model.CurrentSize));
            return View(model);
        }

        /// <summary>
        /// Search page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EntityAuthorize(EntityAuthorizeAction.View)]
        public virtual Task<ActionResult> Search(string actionName = "Index")
        {
            return Task.Run<ActionResult>(() =>
            {
                if (!Metadata.ViewRoles.All(t => User.IsInRole(t)))
                    return new HttpUnauthorizedResult();
                EntitySearchModel<TEntity> model = new EntitySearchModel<TEntity>();
                ViewBag.Action = actionName;
                return View(model);
            });
        }

        /// <summary>
        /// ValueFilterPage.
        /// </summary>
        /// <param name="property">Dependency property.</param>
        /// <param name="value">Value of dependency property.</param>
        /// <param name="selected">Value of selected.</param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<ActionResult> ValueFilter(string property, string value, string selected = null)
        {
            return Task.Run<ActionResult>(() =>
            {

                IPropertyMetadata p = Metadata.GetProperty(property);
                if (p == null)
                    return new HttpStatusCodeResult(404);
                ValueFilterAttribute filterAttribute = p.GetAttribute<ValueFilterAttribute>();
                if (filterAttribute == null)
                    return new HttpStatusCodeResult(400);
                IValueFilter filter = (IValueFilter)Resolver.GetService(filterAttribute.ValueFilter);
                ViewBag.Selected = selected;
                ViewBag.IsRequired = p.IsRequired;
                var collection = filter.GetValues(filterAttribute.DependencyProperty, value);
                return View(collection);
            });
        }
    }
}
