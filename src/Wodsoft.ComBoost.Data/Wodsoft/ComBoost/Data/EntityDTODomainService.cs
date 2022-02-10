using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Data
{
    public class EntityDTODomainService<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> : DomainService
        where TListDTO : class, IEntityDTO
        where TCreateDTO : class, IEntityDTO
        where TEditDTO : class, IEntityDTO
        where TRemoveDTO : class
    {
        #region List

        [EntityViewModelFilter]
        public virtual async Task<IViewModel<TListDTO>> List([FromService] IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> dtoContext)
        {
            var queryable = dtoContext.Query();
            var e = new EntityQueryEventArgs<TListDTO>(queryable);
            await RaiseEvent(e);
            queryable = e.Queryable;
            bool isOrdered = e.IsOrdered;
            OnListQuery(ref queryable, ref isOrdered);
            if (!isOrdered)
                queryable = queryable.OrderByDescending(t => t.CreationDate);
            ViewModel<TListDTO> model = new ViewModel<TListDTO>(queryable);
            await RaiseEvent(new EntityQueryModelCreatedEventArgs<TListDTO>(model));
            return model;
        }

        protected virtual void OnListQuery(ref IQueryable<TListDTO> queryable, ref bool isOrdered)
        {

        }

        #endregion

        #region Create

        public virtual async Task<IUpdateModel<TCreateDTO>> Create([FromService] IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> dtoContext, [FromValue] TCreateDTO dto)
        {
            var validationContext = new ValidationContext(dto, Context.DomainContext, null);
            UpdateModel<TCreateDTO> model = new UpdateModel<TCreateDTO>(dto);
            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, validationContext, results, true))
            {
                model.IsSuccess = false;
                foreach (var result in results)
                    model.ErrorMessage.Add(new KeyValuePair<string, string>(result.MemberNames.FirstOrDefault() ?? string.Empty, result.ErrorMessage));
                return model;
            }
            await RaiseEvent(new EntityPreCreateEventArgs<TCreateDTO>(dto));
            await dtoContext.Add(dto);
            await RaiseEvent(new EntityCreatedEventArgs<TCreateDTO>(dto));
            model.IsSuccess = true;
            return model;
        }

        public virtual async Task<IUpdateRangeModel<TCreateDTO>> CreateRange([FromService] IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> dtoContext, [FromValue] TCreateDTO[] dtos)
        {
            UpdateRangeModel<TCreateDTO> model = new UpdateRangeModel<TCreateDTO>();
            foreach (var dto in dtos)
            {
                var validationContext = new ValidationContext(dto, Context.DomainContext, null);
                List<ValidationResult> results = new List<ValidationResult>();
                if (Validator.TryValidateObject(dto, validationContext, results, true))
                {
                    await RaiseEvent(new EntityPreCreateEventArgs<TCreateDTO>(dto));
                    model.AddItem(dto);
                }
                else
                {
                    model.AddItem(dto, results.Select(t => new KeyValuePair<string, string>(t.MemberNames.FirstOrDefault() ?? string.Empty, t.ErrorMessage)).ToList());
                }
            }
            if (!model.IsSuccess)
                return model;
            await dtoContext.AddRange(dtos);
            foreach (var dto in dtos)
            {
                await RaiseEvent(new EntityCreatedEventArgs<TCreateDTO>(dto));
            }
            return model;
        }

        #endregion

        #region Edit

        public virtual async Task<IUpdateModel<TEditDTO>> Edit([FromService] IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> dtoContext, [FromValue] TEditDTO dto)
        {
            var validationContext = new ValidationContext(dto, Context.DomainContext, null);
            UpdateModel<TEditDTO> model = new UpdateModel<TEditDTO>(dto);
            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, validationContext, results, true))
            {
                model.IsSuccess = false;
                foreach (var result in results)
                    model.ErrorMessage.Add(new KeyValuePair<string, string>(result.MemberNames.FirstOrDefault() ?? string.Empty, result.ErrorMessage));
                return model;
            }
            await RaiseEvent(new EntityPreEditEventArgs<TEditDTO>(dto));
            await dtoContext.Update(dto);
            await RaiseEvent(new EntityEditedEventArgs<TEditDTO>(dto));
            model.IsSuccess = true;
            return model;
        }

        public virtual async Task<IUpdateRangeModel<TEditDTO>> EditRange([FromService] IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> dtoContext, [FromValue] TEditDTO[] dtos)
        {
            UpdateRangeModel<TEditDTO> model = new UpdateRangeModel<TEditDTO>();
            foreach (var dto in dtos)
            {
                var validationContext = new ValidationContext(dto, Context.DomainContext, null);
                List<ValidationResult> results = new List<ValidationResult>();
                if (Validator.TryValidateObject(dto, validationContext, results, true))
                {
                    await RaiseEvent(new EntityPreEditEventArgs<TEditDTO>(dto));
                    model.AddItem(dto);
                }
                else
                {
                    model.AddItem(dto, results.Select(t => new KeyValuePair<string, string>(t.MemberNames.FirstOrDefault() ?? string.Empty, t.ErrorMessage)).ToList());
                }
            }
            if (!model.IsSuccess)
                return model;
            await dtoContext.UpdateRange(dtos);
            foreach (var dto in dtos)
            {
                await RaiseEvent(new EntityEditedEventArgs<TEditDTO>(dto));
            }
            return model;
        }

        #endregion

        #region Remove

        public virtual Task Remove([FromService] IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> dtoContext, [FromValue] TRemoveDTO dto)
        {
            return dtoContext.Remove(dto);
        }

        public virtual Task RemoveRange([FromService] IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> dtoContext, [FromValue] TRemoveDTO[] dtos)
        {
            return dtoContext.RemoveRange(dtos);
        }

        #endregion
    }
}
