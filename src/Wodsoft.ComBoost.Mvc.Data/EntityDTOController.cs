using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityDTOController<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> : Controller
        where TListDTO : class, IEntityDTO
        where TCreateDTO : class, IEntityDTO
        where TEditDTO : class, IEntityDTO
        where TRemoveDTO : class, IEntityDTO
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
            var domain = HttpContext.RequestServices.GetRequiredService<IEntityDTODomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
            domain.Context.EventManager.AddEventHandler<EntityQueryEventArgs<TListDTO>>((context, e) =>
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

        protected virtual void OnListQuery(ref IAsyncQueryable<TListDTO> queryable, ref bool isOrdered) { }

        protected virtual IActionResult OnListModelCreated(IViewModel<TListDTO> model)
        {
            return Ok(model);
        }

        #endregion

        #region Create

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TCreateDTO dto)
        {
            var model = await CreateEntity(dto);
            return OnCreateModelCreated(model);
        }

        protected virtual Task<IUpdateModel<TCreateDTO>> CreateEntity(TCreateDTO dto)
        {
            var domain = HttpContext.RequestServices.GetRequiredService<IEntityDTODomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
            return domain.Create(dto);
        }

        protected virtual IActionResult OnCreateModelCreated(IUpdateModel<TCreateDTO> model)
        {
            return Ok(model);
        }

        #endregion

        #region Edit

        [HttpPut]
        public virtual async Task<IActionResult> Edit([FromBody] TEditDTO dto)
        {
            var model = await EditEntity(dto);
            return OnEditModelCreated(model);
        }

        protected virtual Task<IUpdateModel<TEditDTO>> EditEntity(TEditDTO dto)
        {
            var domain = HttpContext.RequestServices.GetRequiredService<IEntityDTODomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
            return domain.Edit(dto);
        }

        protected virtual IActionResult OnEditModelCreated(IUpdateModel<TEditDTO> model)
        {
            return Ok(model);
        }

        #endregion

        #region Remove

        [HttpDelete]
        public virtual async Task<IActionResult> Remove([FromBody] TRemoveDTO dto)
        {
            var model = await RemoveEntity(dto);
            return OnRemoveModelCreated(model);
        }

        protected virtual Task<IUpdateModel> RemoveEntity(TRemoveDTO dto)
        {
            var domain = HttpContext.RequestServices.GetRequiredService<IEntityDTODomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
            return domain.Remove(dto);
        }

        protected virtual IActionResult OnRemoveModelCreated(IUpdateModel model)
        {
            return Ok(model);
        }

        #endregion
    }
}
