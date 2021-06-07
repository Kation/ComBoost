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

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityController<TKey, TEntity, TListDTO, TCreateDTO, TEditDTO> : Controller
        where TEntity : class, IEntity<TKey>
        where TListDTO : class, IEntityDTO<TKey>
        where TCreateDTO : class, IEntityDTO<TKey>
        where TEditDTO : class, IEntityDTO<TKey>
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
            var domain = HttpContext.RequestServices.GetRequiredService<IEntityDomainTemplate<TKey, TListDTO, TCreateDTO, TEditDTO>>();
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

        protected virtual void OnListQuery(ref IAsyncQueryable<TEntity> queryable, ref bool isOrdered) { }

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
            var domain = HttpContext.RequestServices.GetRequiredService<IEntityDomainTemplate<TKey, TListDTO, TCreateDTO, TEditDTO>>();
            return domain.Create(dto);
        }

        protected virtual IActionResult OnCreateModelCreated(IUpdateModel<TCreateDTO> model)
        {
            return View(model);
        }

        #endregion

        #region Edit

        [HttpGet]
        public virtual async Task<IActionResult> Edit(TKey id)
        {
            var model = await CreateEditModel(id);
            return OnEditModelCreated(model);
        }

        protected virtual async Task<IEditModel<TEditDTO>> CreateEditModel(TKey id)
        {
            var entityContext = HttpContext.RequestServices.GetRequiredService<IEntityContext<TEntity>>();
            var mapper = HttpContext.RequestServices.GetRequiredService<IMapper>();
            var entity = await entityContext.Query().Where(t => t.Id.Equals(id)).FirstOrDefaultAsync();
            var dto = mapper.Map<TEditDTO>(entity);
            IEditModel<TEditDTO> model = new EditModel<TEditDTO>(dto);
            return model;
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
            var domain = HttpContext.RequestServices.GetRequiredService<IEntityDomainTemplate<TKey, TListDTO, TCreateDTO, TEditDTO>>();
            return domain.Edit(dto);
        }

        protected virtual IActionResult OnEditModelCreated(IUpdateModel<TEditDTO> model)
        {
            return View(model);
        }

        #endregion

        #region Remove

        [HttpPost]
        public virtual async Task<IActionResult> Remove(TKey id)
        {
            var model = await RemoveEntity(id);
            return OnRemoveModelCreated(model);
        }

        protected virtual Task<IUpdateModel> RemoveEntity(TKey id)
        {
            var domain = HttpContext.RequestServices.GetRequiredService<IEntityDomainTemplate<TKey, TListDTO, TCreateDTO, TEditDTO>>();
            return domain.Remove(id);
        }

        protected virtual IActionResult OnRemoveModelCreated(IUpdateModel model)
        {
            return View(model);
        }

        #endregion
    }
}
