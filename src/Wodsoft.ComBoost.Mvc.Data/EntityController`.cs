using System;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Security;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.ExceptionServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityController<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> : Controller
        where TEntity : class, IEntity
        where TListDTO : class, IEntityDTO
        where TCreateDTO : class, IEntityDTO
        where TEditDTO : class, IEntityDTO
        where TRemoveDTO : class
    {
        #region List

        [HttpGet]
        public virtual async Task<IActionResult> Index()
        {
            var model = await CreateListModel();
            return OnListModelCreated(model);
        }

        protected virtual async Task<IViewModel<TListDTO>> CreateListModel()
        {
            var domain = HttpContext.RequestServices.GetRequiredService<IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
            domain.Context.EventManager.AddEventHandler<EntityQueryEventArgs<TEntity>>((context, e) =>
            {
                bool isOrdered = e.IsOrdered;
                var queryable = e.Queryable;
                OnListQuery(ref queryable, ref isOrdered);
                e.Queryable = queryable;
                return Task.CompletedTask;
            });
            var model = await domain.List();
            return model;
        }

        protected virtual void OnListQuery(ref IQueryable<TEntity> queryable, ref bool isOrdered) { }

        protected virtual IActionResult OnListModelCreated(IViewModel<TListDTO> model)
        {
            return View(model);
        }

        #endregion

        #region Create

        [HttpGet]
        public virtual async Task<IActionResult> Create()
        {
            var model = await CreateCreateModel();
            return OnCreateModelCreated(model);
        }

        protected virtual Task<IEditModel<TCreateDTO>> CreateCreateModel()
        {
            var entityContext = HttpContext.RequestServices.GetRequiredService<IEntityContext<TEntity>>();
            var mapper = HttpContext.RequestServices.GetRequiredService<IMapper>();
            var entity = entityContext.Create();
            var dto = mapper.Map<TCreateDTO>(entity);
            IEditModel<TCreateDTO> model = new EditModel<TCreateDTO>(dto);
            return Task.FromResult(model);
        }

        protected virtual IActionResult OnCreateModelCreated(IEditModel<TCreateDTO> model)
        {
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(TCreateDTO dto)
        {
            var model = await CreateEntity(dto);
            return OnCreateModelCreated(model);
        }

        protected virtual Task<IUpdateModel<TCreateDTO>> CreateEntity(TCreateDTO dto)
        {
            var domain = HttpContext.RequestServices.GetRequiredService<IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
            return domain.Create(dto);
        }

        protected virtual IActionResult OnCreateModelCreated(IUpdateModel<TCreateDTO> model)
        {
            return View(model);
        }

        #endregion

        #region Edit

        [HttpGet]
        public virtual async Task<IActionResult> Edit()
        {
            var model = await CreateEditModel();
            return OnEditModelCreated(model);
        }

        protected virtual async Task<IEditModel<TEditDTO>> CreateEditModel()
        {
            var entityContext = HttpContext.RequestServices.GetRequiredService<IEntityContext<TEntity>>();
            var mapper = HttpContext.RequestServices.GetRequiredService<IMapper>();
            var entity = await entityContext.GetAsync(await GetKeysFromQuery());
            var dto = mapper.Map<TEditDTO>(entity);
            IEditModel<TEditDTO> model = new EditModel<TEditDTO>(dto);
            return model;
        }

        protected virtual async Task<object[]> GetKeysFromQuery()
        {
            var keyProperties = EntityDescriptor.GetMetadata<TEntity>().KeyProperties;
            var keys = new object[keyProperties.Count];
            var valueProvider = await CompositeValueProvider.CreateAsync(ControllerContext);
            for (int i = 0; i < keyProperties.Count; i++)
            {
                var property = keyProperties[i];
                ModelBinderFactoryContext context = new ModelBinderFactoryContext();
                context.BindingInfo = new BindingInfo() { BinderModelName = property.ClrName, BindingSource = BindingSource.ModelBinding };
                context.CacheToken = property.ClrName + "_" + property.ClrType.Name;
                context.Metadata = MetadataProvider.GetMetadataForType(property.ClrType);
                var binder = ModelBinderFactory.CreateBinder(context);
                var bindingContext = DefaultModelBindingContext.CreateBindingContext(ControllerContext, valueProvider, context.Metadata, context.BindingInfo, property.ClrName);
                try
                {
                    binder.BindModelAsync(bindingContext).Wait();
                    if (bindingContext.Result.IsModelSet)
                        keys[i] = bindingContext.Result.Model;
                }
                catch
                {

                }
            }
            return keys;
        }

        protected virtual IActionResult OnEditModelCreated(IEditModel<TEditDTO> model)
        {
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit(TEditDTO dto)
        {
            var model = await EditEntity(dto);
            return OnEditModelCreated(model);
        }

        protected virtual Task<IUpdateModel<TEditDTO>> EditEntity(TEditDTO dto)
        {
            var domain = HttpContext.RequestServices.GetRequiredService<IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
            return domain.Edit(dto);
        }

        protected virtual IActionResult OnEditModelCreated(IUpdateModel<TEditDTO> model)
        {
            return View(model);
        }

        #endregion

        #region Remove

        [HttpPost]
        public virtual async Task<IActionResult> Remove(TRemoveDTO dto)
        {
            await RemoveEntity(dto);
            return NoContent();
        }

        protected virtual Task RemoveEntity(TRemoveDTO dto)
        {
            var domain = HttpContext.RequestServices.GetRequiredService<IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
            return domain.Remove(dto);
        }

        #endregion
    }
}
