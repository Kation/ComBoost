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

namespace System.Web.Mvc
{
    public class EntityControllerUnitils<TEntity> where TEntity : class, IEntity, new()
    {
        public EntityControllerUnitils(Controller controller, IEntityContextBuilder builder)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");
            if (builder == null)
                throw new ArgumentNullException("builder");
            Controller = controller;
            EntityBuilder = builder;
            EntityQueryable = builder.GetContext<TEntity>();
            Metadata = EntityAnalyzer.GetMetadata<TEntity>();
        }

        public Controller Controller { get; private set; }

        public IEntityContextBuilder EntityBuilder { get; private set; }

        public IEntityQueryable<TEntity> EntityQueryable { get; private set; }

        public IEntityMetadata Metadata { get; private set; }

        public int[] PageSize { get; set; }

        #region Index

        public async Task<ActionResult> GetIndexAction(GetIndexModelDelegate modelDelegate, GetSearchItemDelegate searchDelegate, GetParentModelDelegate parentDelegate, int page = 1, int size = 20, string parentpath = null, Guid? parentid = null, bool search = false)
        {
            if (page < 1)
                return new HttpStatusCodeResult(400);
            if (size < 1)
                return new HttpStatusCodeResult(400);
            IQueryable<TEntity> queryable = EntityQueryable.Query();
            EntitySearchItem[] searchItems;
            if (search)
            {
                searchItems = searchDelegate(ref queryable);
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
            var model = await modelDelegate(queryable, page, size);
            if (model == null)
                return new HttpStatusCodeResult(404);
            if (Metadata.ParentProperty != null && !search)
                model.Parent = await parentDelegate(parentid);
            model.SearchItem = searchItems;
            model.Items = await EntityQueryable.ToArrayAsync(model.Queryable.Skip((model.CurrentPage - 1) * model.CurrentSize).Take(model.CurrentSize));
            if (model.ViewButtons.Length > 0)
            {
                IServiceProvider serviceProvider = Controller.GetServiceProvider();
                foreach (var item in model.ViewButtons)
                    item.SetTarget(serviceProvider);
            }
            return GetView(model);
        }

        public async Task<EntityViewModel<TEntity>> GetIndexModel(IQueryable<TEntity> queryable, int page, int size)
        {
            return await Task.Run<EntityViewModel<TEntity>>(() =>
            {
                var model = new EntityViewModel<TEntity>(EntityQueryable.OrderBy(queryable), page, size);
                model.Headers = Metadata.ViewProperties.Where(t => (t.AllowAnonymous || Controller.User.Identity.IsAuthenticated) && !t.ViewRoles.Any(r => !Controller.User.IsInRole(r))).ToArray();
                model.PageSizeOption = PageSize;
                return model;
            });
        }

        public delegate Task<EntityViewModel<TEntity>> GetIndexModelDelegate(IQueryable<TEntity> queryable, int page, int size);

        /// <summary>
        /// Get search item in request.
        /// </summary>
        /// <param name="queryable">Queryable of TEntity.</param>
        /// <returns>Search item of TEntity.</returns>
        public EntitySearchItem[] GetSearchItem(ref IQueryable<TEntity> queryable)
        {
            List<EntitySearchItem> searchItems = new List<EntitySearchItem>();
            var keys = Controller.Request.QueryString.AllKeys.Where(t => t.StartsWith("Search.")).Select(t => t.Substring(7).Split('.')).GroupBy(t => t[0], t => t.Length == 1 ? "" : "." + t[1]).ToArray();
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
                                if (!DateTime.TryParse(Controller.Request.QueryString["Search." + keys[i].Key + options[a]], out start))
                                    continue;
                                searchItem.MorethanDate = start;
                                ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                                queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.GreaterThanOrEqual(Expression.Property(parameter, property.ClrName), Expression.Constant(start)), parameter));
                            }
                            else if (options[a] == ".End")
                            {
                                DateTime end;
                                if (!DateTime.TryParse(Controller.Request.QueryString["Search." + keys[i].Key + options[a]], out end))
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
                            if (!bool.TryParse(Controller.Request.QueryString["Search." + keys[i].Key], out result))
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
                                if (!double.TryParse(Controller.Request.QueryString["Search." + keys[i].Key + options[a]], out start))
                                    continue;
                                searchItem.Morethan = start;
                                ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                                queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(Expression.GreaterThanOrEqual(Expression.Property(parameter, property.ClrName), Expression.Constant(start)), parameter));
                            }
                            else if (options[a] == ".End")
                            {
                                double end;
                                if (!double.TryParse(Controller.Request.QueryString["Search." + keys[i].Key + options[a]], out end))
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
                                result = Enum.Parse(property.ClrType, Controller.Request.QueryString["Search." + keys[i].Key]);
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
                            searchItem.Contains = Controller.Request.QueryString["Search." + keys[i].Key];
                            ParameterExpression parameter = Expression.Parameter(Metadata.Type);
                            Expression expression = Expression.Property(Expression.Property(parameter, property.ClrName), EntityAnalyzer.GetMetadata(property.ClrType).DisplayProperty.ClrName);
                            expression = Expression.Call(expression, typeof(string).GetMethod("Contains"), Expression.Constant(searchItem.Contains));
                            queryable = queryable.Where<TEntity>(Expression.Lambda<Func<TEntity, bool>>(expression, parameter));
                        }
                        break;
                    default:
                        if (property.ClrType == typeof(string))
                        {
                            searchItem.Contains = Controller.Request.QueryString["Search." + keys[i].Key];
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

        public delegate EntitySearchItem[] GetSearchItemDelegate(ref IQueryable<TEntity> queryable);

        /// <summary>
        /// Get parent model.
        /// </summary>
        /// <param name="selected">Selected parent id.</param>
        /// <returns>Parent model.</returns>
        public async Task<EntityParentModel[]> GetParentModel(Guid? selected)
        {
            List<EntityParentModel> root = new List<EntityParentModel>();
            await GetParentModel(root, new Dictionary<Guid, EntityParentModel[]>(), Metadata, selected, Metadata.ParentProperty.ClrName);
            return root.ToArray();
        }

        public delegate Task<EntityParentModel[]> GetParentModelDelegate(Guid? selected);

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

        private static MethodInfo _ESelectMethod = typeof(Linq.Enumerable).GetMethods().Where(t => t.Name == "Select" && t.GetParameters().Length == 2).First();
        private static MethodInfo _QSelectMethod = typeof(Linq.Queryable).GetMethods().Where(t => t.Name == "Select" && t.GetParameters().Length == 2).First();
        private static MethodInfo _GroupByMethod = typeof(Linq.Queryable).GetMethods().Where(t => t.Name == "GroupBy" && t.GetParameters().Length == 2).First();
        private object GetGrouping(object queryable, Type tkey, Type tsource, Expression expression)
        {
            return _GroupByMethod.MakeGenericMethod(tsource, tkey).Invoke(null, new object[] { queryable, expression });
        }

        #endregion

        #region Create

        /// <summary>
        /// Create entity page.
        /// </summary>
        /// <param name="parent">Parent id.</param>
        /// <returns></returns>
        public async Task<ActionResult> GetCreateAction(GetCreateModelDelegate modelDelegate, Guid? parent = null)
        {
            if (!EntityQueryable.Addable())
                return new HttpStatusCodeResult(403);
            var model = await modelDelegate(parent);
            if (model == null)
                return new HttpStatusCodeResult(404);
            return GetView("Edit", model);
        }

        /// <summary>
        /// Get create action model.
        /// </summary>
        /// <param name="parent">Parent id.</param>
        /// <returns>Edit model with new entity item of TEntity.</returns>
        public async Task<EntityEditModel<TEntity>> GetCreateModel(Guid? parent = null)
        {
            var model = new EntityEditModel<TEntity>(EntityQueryable.Create());
            model.Item.Index = Guid.Empty;
            model.Properties = Metadata.CreateProperties.Where(t => (t.AllowAnonymous || Controller.User.Identity.IsAuthenticated) && !t.EditRoles.Any(r => !Controller.User.IsInRole(r))).ToArray();
            if (parent != null && model.Metadata.ParentProperty != null)
            {
                dynamic parentContext = EntityBuilder.GetContext(model.Metadata.ParentProperty.ClrType);
                object parentObj = await parentContext.GetEntityAsync(parent.Value);
                model.Metadata.ParentProperty.SetValue(model.Item, parentObj);
            }
            return model;
        }

        public delegate Task<EntityEditModel<TEntity>> GetCreateModelDelegate(Guid? parent = null);

        #endregion

        #region Detail

        /// <summary>
        /// Entity detail page.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        public async Task<ActionResult> GetDetailAction(GetDetailModelDelegate modelDelegate, Guid id)
        {
            var model = await modelDelegate(id);
            if (model == null)
                return new HttpStatusCodeResult(404);
            return GetView(model);
        }

        /// <summary>
        /// Get detail action model.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Edit model with detail properties of TEntity.</returns>
        public async Task<EntityEditModel<TEntity>> GetDetailModel(Guid id)
        {
            TEntity item = await EntityQueryable.GetEntityAsync(id);
            if (item == null)
                return null;
            var model = new EntityEditModel<TEntity>(item);
            model.Properties = Metadata.DetailProperties;
            return model;
        }

        public delegate Task<EntityEditModel<TEntity>> GetDetailModelDelegate(Guid id);

        #endregion

        #region Edit

        /// <summary>
        /// Edit entity page.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        public async Task<ActionResult> GetEditAction(GetEditModelDelegate modelDelegate, Guid id)
        {
            if (!EntityQueryable.Editable())
                return new HttpStatusCodeResult(403);
            var model = await modelDelegate(id);
            if (model == null)
                return new HttpStatusCodeResult(404);
            return GetView(model);
        }

        /// <summary>
        /// Get edit action model.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Edit model of TEntity.</returns>
        public async Task<EntityEditModel<TEntity>> GetEditModel(Guid id)
        {
            TEntity item = await EntityQueryable.GetEntityAsync(id);
            if (item == null)
                return null;
            var model = new EntityEditModel<TEntity>(item);
            model.Properties = Metadata.EditProperties.Where(t => (t.AllowAnonymous || Controller.User.Identity.IsAuthenticated) && !t.EditRoles.Any(r => !Controller.User.IsInRole(r))).ToArray();
            return model;
        }

        public delegate Task<EntityEditModel<TEntity>> GetEditModelDelegate(Guid id);

        #endregion

        #region Remove

        /// <summary>
        /// Remove entity.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        public async Task<ActionResult> GetRemoveAction(RemoveCoreDelegate coreDelegate, Guid id)
        {
            if (!EntityQueryable.Removeable())
                return new HttpStatusCodeResult(403);
            if (await coreDelegate(id))
                return new HttpStatusCodeResult(200);
            else
                return new HttpStatusCodeResult(404);
        }

        /// <summary>
        /// Remoce action core method.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>True if success, otherwise is false.</returns>
        public async Task<bool> RemoveCore(Guid id)
        {
            if (await EntityQueryable.RemoveAsync(id))
                return true;
            else
                return false;
        }

        public delegate Task<bool> RemoveCoreDelegate(Guid id);

        #endregion

        #region Update

        public async Task<ActionResult> GetUpdateAction(UpdateCoreDelegate coreDelegate, UpdatePropertyDelegate propertyDelegate, Guid id)
        {
            TEntity entity;
            if (id == Guid.Empty)
            {
                if (!EntityQueryable.Addable())
                    return new HttpUnauthorizedResult();
                if (!Metadata.AddRoles.All(t => Controller.User.IsInRole(t)))
                    return new HttpUnauthorizedResult();
                entity = EntityQueryable.Create();
            }
            else
            {
                if (!EntityQueryable.Editable())
                    return new HttpUnauthorizedResult();
                if (!Metadata.EditRoles.All(t => Controller.User.IsInRole(t)))
                    return new HttpUnauthorizedResult();
                entity = EntityQueryable.GetEntity(id);
                if (entity == null)
                    return new HttpStatusCodeResult(404);
            }
            var properties = Metadata.Properties.Where(t => !t.IsHiddenOnEdit).ToArray();
            if (!await coreDelegate(propertyDelegate, entity))
            {
                Controller.Response.StatusCode = 400;
                //Important!!!
                Controller.Response.TrySkipIisCustomErrors = true;
                if (Controller.ViewBag.ErrorMessage == null)
                    return GetContent("未知");
                else
                    return GetContent(Controller.ViewBag.ErrorMessage);
            }
            return GetContent(entity.Index.ToString());
        }

        /// <summary>
        /// Update action core method.
        /// </summary>
        /// <param name="entity">Entity to edit or create.</param>
        /// <returns>True if success, otherwise is false.</returns>
        public async Task<bool> UpdateCore(UpdatePropertyDelegate propertyDelegate, TEntity entity)
        {
            var properties = Metadata.EditProperties.ToArray();
            foreach (var property in properties)
            {
                await propertyDelegate(entity, property);
            }
            if (Controller.ViewBag.ErrorMessage != null)
                return false;
            ValidationContext validationContext = new ValidationContext(entity, new EntityDescriptorContext(EntityBuilder), null);
            var validateResult = entity.Validate(validationContext);
            if (validateResult.Count() != 0)
            {
                Controller.ViewBag.ErrorMessage = string.Join("\r\n", validateResult.Select(t => t.ErrorMessage));
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
        public async Task UpdateProperty(TEntity entity, IPropertyMetadata propertyMetadata)
        {
            if (propertyMetadata.EditRoles.Any(t => !Controller.User.IsInRole(t)))
                return;
            if (propertyMetadata.IsFileUpload)
            {
                #region File Path Value
                if (!Controller.Request.Files.AllKeys.Contains(propertyMetadata.ClrName))
                    return;
                if (!(Controller is IFileController<TEntity>))
                {
                    if (propertyMetadata.ClrType != typeof(byte[]))
                        throw new NotSupportedException("Controller doesn't support upload file.");
                    if (Controller.Request.Files.AllKeys.Contains(propertyMetadata.ClrName))
                    {
                        var file = Controller.Request.Files[propertyMetadata.ClrName];
                        MemoryStream stream = new MemoryStream();
                        await file.InputStream.CopyToAsync(stream);
                        propertyMetadata.SetValue(entity, stream.ToArray());
                        stream.Close();
                        stream.Dispose();
                    }
                }
                else
                    await ((IFileController<TEntity>)Controller).SaveFileToProperty(entity, propertyMetadata, Controller.Request.Files[propertyMetadata.ClrName]);
                #endregion
            }
            else
            {
                if (!Controller.Request.Form.AllKeys.Contains(propertyMetadata.ClrName))
                    return;
                string value = Controller.Request.Form[propertyMetadata.ClrName];
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
                        Controller.ViewBag.ErrorMessage = ex.Message;
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

        public delegate Task<bool> UpdateCoreDelegate(UpdatePropertyDelegate propertyDelegate, TEntity entity);

        public delegate Task UpdatePropertyDelegate(TEntity entity, IPropertyMetadata propertyMetadata);

        #endregion

        #region Other

        /// <summary>
        /// Selector page.
        /// </summary>
        /// <param name="page">Number of current page.</param>
        /// <param name="size">Number of entities per page.</param>
        /// <param name="parentpath">Path of parent for entity.</param>
        /// <param name="parentid">Parent id.</param>
        /// <param name="search">Is a search result.</param>
        /// <returns></returns>
        public async Task<ActionResult> GetSelectorAction(GetIndexModelDelegate modelDelegate, GetSearchItemDelegate searchDelegate, GetParentModelDelegate parentDelegate, int page = 1, int size = 10, string parentpath = null, Guid? parentid = null, bool search = false)
        {
            IQueryable<TEntity> queryable = EntityQueryable.Query();
            EntitySearchItem[] searchItems = null;
            if (search)
            {
                searchItems = searchDelegate(ref queryable);
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
            var model = await modelDelegate(EntityQueryable.OrderBy(queryable), page, size);
            if (model == null)
                return new HttpStatusCodeResult(404);
            if (Metadata.ParentProperty != null)
                model.Parent = await parentDelegate(parentid);
            model.Headers = Metadata.ViewProperties;
            model.SearchItem = searchItems;
            model.Items = await EntityQueryable.ToArrayAsync(model.Queryable.Skip((model.CurrentPage - 1) * model.CurrentSize).Take(model.CurrentSize));
            return GetView(model);
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
        public async Task<ActionResult> GetMultipleSelectorAction(GetIndexModelDelegate modelDelegate, GetSearchItemDelegate searchDelegate, GetParentModelDelegate parentDelegate, int page = 1, int size = 10, string parentpath = null, Guid? parentid = null, bool search = false)
        {
            IQueryable<TEntity> queryable = EntityQueryable.Query();
            EntitySearchItem[] searchItems = null;
            if (search)
            {
                searchItems = searchDelegate(ref queryable);
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
            var model = await modelDelegate(EntityQueryable.OrderBy(queryable), page, size);
            if (model == null)
                return new HttpStatusCodeResult(404);
            if (Metadata.ParentProperty != null)
                model.Parent = await parentDelegate(parentid);
            model.Headers = Metadata.ViewProperties;
            model.SearchItem = searchItems;
            model.Items = await EntityQueryable.ToArrayAsync(model.Queryable.Skip((model.CurrentPage - 1) * model.CurrentSize).Take(model.CurrentSize));
            return GetView(model);
        }

        /// <summary>
        /// Search page.
        /// </summary>
        /// <returns></returns>
        public virtual Task<ActionResult> GetSearchAction(string actionName = "Index")
        {
            return Task.Run<ActionResult>(() =>
            {
                if (!Metadata.ViewRoles.All(t => Controller.User.IsInRole(t)))
                    return new HttpUnauthorizedResult();
                EntitySearchModel<TEntity> model = new EntitySearchModel<TEntity>();
                Controller.ViewBag.Action = actionName;
                return GetView(model);
            });
        }

        #endregion

        private ActionResult GetView(object model)
        {
            return GetView(null, model);
        }

        private ActionResult GetView(string viewName, object model)
        {
            if (model != null)
                Controller.ViewData.Model = model;
            return new ViewResult
            {
                ViewName = viewName,
                ViewData = Controller.ViewData,
                TempData = Controller.TempData,
                ViewEngineCollection = Controller.ViewEngineCollection
            };
        }

        private ActionResult GetContent(string content)
        {
            return GetContent(content, null, null);
        }

        private ActionResult GetContent(string content, string contentType)
        {
            return GetContent(content, contentType, null);
        }

        private ActionResult GetContent(string content, string contentType, Encoding contentEncoding)
        {
            return new ContentResult
            {
                Content = content,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }
    }
}
