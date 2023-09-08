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
    public abstract class EntityApiController<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO, TListResult, TCreateResult, TEditResult, TRemoveResult> : ControllerBase
        where TEntity : class, IEntity
        where TListDTO : class
        where TCreateDTO : class
        where TEditDTO : class
        where TRemoveDTO : class
    {
        #region List

        public virtual async Task<TListResult> List()
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

        protected abstract Task<TListResult> HandleListModelAsync(IViewModel<TListDTO> viewModel);

        #endregion

        #region Create

        public virtual async Task<TCreateResult> Create(TCreateDTO dto)
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

        protected abstract Task<TCreateResult> HandleCreateModelAsync(IUpdateModel<TListDTO> model, TCreateDTO createDto);

        #endregion

        #region Edit

        public virtual async Task<TEditResult> Edit(TEditDTO dto)
        {
            var model = await EditEntity(dto);
            return await OnEditModelCreated(model, dto);
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

        protected abstract Task<TEditResult> OnEditModelCreated(IUpdateModel<TListDTO> model, TEditDTO editDto);

        #endregion

        #region Remove

        public virtual async Task<TRemoveResult> Remove(TRemoveDTO dto)
        {
            await RemoveEntity(dto);
            return await OnRemoved(dto);
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

        protected abstract Task<TRemoveResult> OnRemoved(TRemoveDTO dto);

        #endregion
    }

    public abstract class CurdController<TEntity, TDTO, TListResult, TCreateResult, TEditResult, TRemoveResult> : EntityApiController<TEntity, TDTO, TDTO, TDTO, TDTO, TListResult, TCreateResult, TEditResult, TRemoveResult>
        where TEntity : class, IEntity
        where TDTO : class
    {

    }
}
