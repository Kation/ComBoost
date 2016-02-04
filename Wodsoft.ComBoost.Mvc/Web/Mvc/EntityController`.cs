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
        /// Get the context of entity.
        /// </summary>
        public IEntityContext<TEntity> EntityContext { get; private set; }

        /// <summary>
        /// Get the context of entity.
        /// </summary>
        [Obsolete("Please use EntityContext to replace EntityQueryable. EntityQueryable will be removed in next major version.")]
        public IEntityContext<TEntity> EntityQueryable { get; private set; }

        /// <summary>
        /// Get the untils of entity controller.
        /// </summary>
        public EntityControllerUnitils<TEntity> Untils { get; private set; }

        /// <summary>
        /// Initialize entity controller.
        /// </summary>
        /// <param name="builder">Context builder of entity.</param>
        public EntityController(IEntityContextBuilder builder)
            : base(builder)
        {
            EntityQueryable = EntityContext = EntityBuilder.GetContext<TEntity>();
            Metadata = EntityAnalyzer.GetMetadata<TEntity>();
            PageSize = Pagination.DefaultPageSizeOption;
            Untils = new EntityControllerUnitils<TEntity>(this, builder);
            Untils.PageSize = PageSize;
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
        public virtual Task<ActionResult> Index(int page = 1, int size = 20, string parentpath = null, Guid? parentid = null, bool search = false)
        {
            return Untils.GetIndexAction(GetIndexModel, GetSearchItem, GetParentModel, page, size, parentpath, parentid, search);
        }

        /// <summary>
        /// Get search item in request.
        /// </summary>
        /// <param name="queryable">Queryable of TEntity.</param>
        /// <returns>Search item of TEntity.</returns>
        protected virtual EntitySearchItem[] GetSearchItem(ref IQueryable<TEntity> queryable)
        {
            return Untils.GetSearchItem(ref queryable);
        }

        /// <summary>
        /// Get index action model.
        /// </summary>
        /// <param name="queryable">Queryable object of TEntity.</param>
        /// <param name="page">Current page.</param>
        /// <param name="size">Current page size.</param>
        /// <returns>View model of TEntity.</returns>
        protected virtual Task<EntityViewModel<TEntity>> GetIndexModel(IQueryable<TEntity> queryable, int page, int size)
        {
            return Untils.GetIndexModel(queryable, page, size);
        }

        /// <summary>
        /// Get parent model.
        /// </summary>
        /// <param name="selected">Selected parent id.</param>
        /// <returns>Parent model.</returns>
        protected virtual Task<EntityParentModel[]> GetParentModel(Guid? selected)
        {
            return Untils.GetParentModel(selected);
        }
        
        /// <summary>
        /// Create entity page.
        /// </summary>
        /// <param name="parent">Parent id.</param>
        /// <returns></returns>
        [HttpGet]
        [EntityAuthorize(EntityAuthorizeAction.Create)]
        public virtual Task<ActionResult> Create(Guid? parent = null)
        {
            return Untils.GetCreateAction(GetCreateModel, parent);
        }

        /// <summary>
        /// Get create action model.
        /// </summary>
        /// <param name="parent">Parent id.</param>
        /// <returns>Edit model with new entity item of TEntity.</returns>
        protected virtual Task<EntityEditModel<TEntity>> GetCreateModel(Guid? parent = null)
        {
            return Untils.GetCreateModel(parent);
        }

        /// <summary>
        /// Entity detail page.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [HttpGet]
        [EntityAuthorize(EntityAuthorizeAction.View)]
        public virtual Task<ActionResult> Detail(Guid id)
        {
            return Untils.GetDetailAction(GetDetailModel, id);
        }

        /// <summary>
        /// Get detail action model.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Edit model with detail properties of TEntity.</returns>
        protected virtual Task<EntityEditModel<TEntity>> GetDetailModel(Guid id)
        {
            return Untils.GetDetailModel(id);
        }

        /// <summary>
        /// Edit entity page.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [HttpGet]
        [EntityAuthorize(EntityAuthorizeAction.Edit)]
        public virtual Task<ActionResult> Edit(Guid id)
        {
            return Untils.GetEditAction(GetEditModel, id);
        }

        /// <summary>
        /// Get edit action model.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Edit model of TEntity.</returns>
        protected virtual Task<EntityEditModel<TEntity>> GetEditModel(Guid id)
        {
            return Untils.GetEditModel(id);
        }

        /// <summary>
        /// Remove entity.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [HttpPost]
        [EntityAuthorize(EntityAuthorizeAction.Remove)]
        public virtual  Task<ActionResult> Remove(Guid id)
        {
            return Untils.GetRemoveAction(RemoveCore, id);
        }

        /// <summary>
        /// Remoce action core method.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>True if success, otherwise is false.</returns>
        protected virtual Task<bool> RemoveCore(Guid id)
        {
            return Untils.RemoveCore(id);
        }

        /// <summary>
        /// Update entity.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        [HttpPost]
        [EntityAuthorize]
        public virtual Task<ActionResult> Update(Guid id)
        {
            return Untils.GetUpdateAction((p, entity) =>
            {
                return UpdateCore(entity);
            }, UpdateProperty, id);
        }
        
        /// <summary>
        /// Update action core method.
        /// </summary>
        /// <param name="entity">Entity to edit or create.</param>
        /// <returns>True if success, otherwise is false.</returns>
        protected virtual Task<bool> UpdateCore(TEntity entity)
        {
            return Untils.UpdateCore(UpdateProperty, entity);
        }

        /// <summary>
        /// Update property for entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <param name="propertyMetadata">Property metadata.</param>
        /// <returns></returns>
        protected virtual Task UpdateProperty(TEntity entity, IPropertyMetadata propertyMetadata)
        {
            return Untils.UpdateProperty(entity, propertyMetadata);
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
        public virtual Task<ActionResult> Selector(int page = 1, int size = 10, string parentpath = null, Guid? parentid = null, bool search = false)
        {
            return Untils.GetSelectorAction(GetIndexModel, GetSearchItem, GetParentModel, page, size, parentpath, parentid, search);
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
        public virtual Task<ActionResult> MultipleSelector(int page = 1, int size = 10, string parentpath = null, Guid? parentid = null, bool search = false)
        {
            return Untils.GetMultipleSelectorAction(GetIndexModel, GetSearchItem, GetParentModel, page, size, parentpath, parentid, search);
        }

        /// <summary>
        /// Search page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EntityAuthorize(EntityAuthorizeAction.View)]
        public virtual Task<ActionResult> Search(string actionName = "Index")
        {
            return Untils.GetSearchAction(actionName);
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
