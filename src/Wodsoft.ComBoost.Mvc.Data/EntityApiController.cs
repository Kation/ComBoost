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

namespace Wodsoft.ComBoost.Mvc
{
    public abstract class EntityApiController<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> : ControllerBase
        where TEntity : class, IEntity
        where TListDTO : class
        where TCreateDTO : class
        where TEditDTO : class
        where TRemoveDTO : class
    {
        #region List

        [HttpGet]
        public virtual async Task<IActionResult> List()
        {
            var model = await GetListModel();
            return await HandleListModelAsync(model);
        }

        protected virtual async Task<IViewModel<TListDTO>> GetListModel()
        {
            var domain = GetListDomainTemplate();
            if (HasQueryFilter)
            {
                domain.Context.EventManager.AddEventHandler<EntityQueryEventArgs<TEntity>>((context, e) =>
                {
                    return QueryFilter(e);
                });
            }
            var model = await domain.List();
            return model;
        }

        protected virtual IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> GetListDomainTemplate()
        {
            return HttpContext.RequestServices.GetRequiredService<IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
        }

        protected virtual bool HasQueryFilter { get; }

        protected virtual Task QueryFilter(EntityQueryEventArgs<TEntity> e)
        {
            return Task.CompletedTask;
        }

        protected virtual Task<IActionResult> HandleListModelAsync(IViewModel<TListDTO> viewModel)
        {
            return Task.FromResult<IActionResult>(Ok(viewModel));
        }

        #endregion

        #region Create

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TCreateDTO dto)
        {
            var model = await CreateEntity(dto);
            return await HandleCreateModelAsync(model, dto);
        }

        protected virtual Task<IUpdateModel<TListDTO>> CreateEntity(TCreateDTO dto)
        {
            var domain = GetCreateDomainTemplate();
            return domain.Create(dto);
        }

        protected virtual IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> GetCreateDomainTemplate()
        {
            return HttpContext.RequestServices.GetRequiredService<IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
        }

        protected virtual Task<IActionResult> HandleCreateModelAsync(IUpdateModel<TListDTO> model, TCreateDTO dto)
        {
            return Task.FromResult<IActionResult>(Ok(model));
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateRange([FromBody] TCreateDTO[] dtos)
        {
            var model = await CreateEntities(dtos);
            return await HandleCreateRangeModelAsync(model, dtos);
        }

        protected virtual Task<IUpdateRangeModel<TListDTO>> CreateEntities(TCreateDTO[] dtos)
        {
            var domain = GetCreateDomainTemplate();
            return domain.CreateRange(dtos);
        }

        protected virtual Task<IActionResult> HandleCreateRangeModelAsync(IUpdateRangeModel<TListDTO> model, TCreateDTO[] dtos)
        {
            return Task.FromResult<IActionResult>(Ok(model));
        }

        #endregion

        #region Edit

        [HttpPut]
        public virtual async Task<IActionResult> Edit([FromBody] TEditDTO dto)
        {
            var model = await EditEntity(dto);
            return await HandleEditModelAsync(model, dto);
        }

        protected virtual Task<IUpdateModel<TListDTO>> EditEntity(TEditDTO dto)
        {
            var domain = GetEditDomainTemplate();
            return domain.Edit(dto);
        }

        protected virtual IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> GetEditDomainTemplate()
        {
            return HttpContext.RequestServices.GetRequiredService<IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
        }

        protected virtual Task<IActionResult> HandleEditModelAsync(IUpdateModel<TListDTO> model, TEditDTO dto)
        {
            return Task.FromResult<IActionResult>(Ok(model));
        }

        [HttpPut]
        public virtual async Task<IActionResult> EditRange([FromBody] TEditDTO[] dtos)
        {
            var model = await EditEntities(dtos);
            return await HandleEditRangeModelAsync(model, dtos);
        }

        protected virtual Task<IUpdateRangeModel<TListDTO>> EditEntities(TEditDTO[] dtos)
        {
            var domain = GetEditDomainTemplate();
            return domain.EditRange(dtos);
        }

        protected virtual Task<IActionResult> HandleEditRangeModelAsync(IUpdateRangeModel<TListDTO> model, TEditDTO[] dtos)
        {
            return Task.FromResult<IActionResult>(Ok(model));
        }

        #endregion

        #region Remove

        [HttpDelete]
        public virtual async Task<IActionResult> Remove([FromBody] TRemoveDTO dto)
        {
            await RemoveEntity(dto);
            return await HandleRemoveModelAsync(dto);
        }

        protected virtual Task RemoveEntity(TRemoveDTO dto)
        {
            var domain = GetRemoveDomainTemplate();
            return domain.Remove(dto);
        }

        protected virtual IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> GetRemoveDomainTemplate()
        {
            return HttpContext.RequestServices.GetRequiredService<IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
        }

        protected virtual Task<IActionResult> HandleRemoveModelAsync(TRemoveDTO dto)
        {
            return Task.FromResult<IActionResult>(Ok());
        }

        [HttpDelete]
        public virtual async Task<IActionResult> RemoveRange([FromBody] TRemoveDTO[] dtos)
        {
            await RemoveEntities(dtos);
            return await HandleRemoveRangeModelAsync(dtos);
        }

        protected virtual Task RemoveEntities(TRemoveDTO[] dtos)
        {
            var domain = GetRemoveDomainTemplate();
            return domain.RemoveRange(dtos);
        }

        protected virtual Task<IActionResult> HandleRemoveRangeModelAsync(TRemoveDTO[] dtos)
        {
            return Task.FromResult<IActionResult>(Ok());
        }

        #endregion
    }
}
