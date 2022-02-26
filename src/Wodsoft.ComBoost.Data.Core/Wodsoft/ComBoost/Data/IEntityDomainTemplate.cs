using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Data
{
    public interface IEntityDomainTemplate<TDto> : IEntityDomainTemplate<TDto, TDto, TDto, TDto>
        where TDto : class
    { }

    public interface IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> : IDomainTemplate
        where TListDTO : class
        where TCreateDTO : class
        where TEditDTO : class
        where TRemoveDTO : class
    {
        Task<IViewModel<TListDTO>> List();

        Task<IViewModel<TListDTO>> List(int page, int size);

        Task<IUpdateModel<TCreateDTO>> Create(TCreateDTO dto);

        Task<IUpdateRangeModel<TCreateDTO>> CreateRange(TCreateDTO[] dtos);

        Task<IUpdateModel<TEditDTO>> Edit(TEditDTO dto);

        Task<IUpdateRangeModel<TEditDTO>> EditRange(TEditDTO[] dtos);

        Task Remove(TRemoveDTO dto);

        Task RemoveRange(TRemoveDTO[] dtos);
    }
}
