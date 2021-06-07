using AutoMapper;
using AutoMapper.QueryableExtensions;
using AutoMapper.QueryableExtensions.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Data
{
    public class EntityDTODomainService<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> : DomainService
        where TListDTO : class, IEntityDTO
        where TCreateDTO : class, IEntityDTO
        where TEditDTO : class, IEntityDTO
        where TRemoveDTO : class, IEntityDTO
    {
        #region List

        [EntityViewModelFilter]
        public virtual async Task<IViewModel<TListDTO>> List([FromService] IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> dtoContext)
        {
            var metadata = EntityDescriptor.GetMetadata<TListDTO>();
            var queryable = dtoContext.Query();
            var e = new EntityQueryEventArgs<TListDTO>(queryable);
            await RaiseEvent(e);
            queryable = e.Queryable;
            bool isOrdered = e.IsOrdered;
            OnListQuery(ref queryable, ref isOrdered);
            if (!isOrdered)
                queryable = queryable.OrderByDescending(t => t.CreationDate);
            ViewModel<TListDTO> model = new ViewModel<TListDTO>(queryable);
            return model;
        }

        protected virtual void OnListQuery(ref IAsyncQueryable<TListDTO> queryable, ref bool isOrdered)
        {

        }

        #endregion

        #region Create

        public virtual async Task<IUpdateModel<TCreateDTO>> Create([FromService] IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> dtoContext, [FromValue] TCreateDTO dto)
        {
            await dtoContext.Add(dto);
            UpdateModel<TCreateDTO> model = new UpdateModel<TCreateDTO>
            {
                IsSuccess = true,
                Result = dto
            };
            return model;
        }

        protected virtual void OnCreateModelCreated(EntityEditModel<TCreateDTO> model)
        {

        }

        #endregion

        #region Edit

        public virtual async Task<IUpdateModel<TEditDTO>> Edit([FromService] IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> dtoContext, [FromValue] TEditDTO dto)
        {
            await dtoContext.Update(dto);
            IUpdateModel<TEditDTO> model = new UpdateModel<TEditDTO>
            {
                IsSuccess = true,
                Result = dto
            };
            return model;
        }

        protected virtual void OnEditModelCreated(EntityEditModel<TEditDTO> model)
        {

        }

        #endregion

        #region Remove

        public virtual async Task<IUpdateModel> Remove([FromService] IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> dtoContext, [FromValue] TRemoveDTO dto)
        {
            await dtoContext.Remove(dto);
            return new UpdateModel() { IsSuccess = true };
        }

        #endregion
    }
}
