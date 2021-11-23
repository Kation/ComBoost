using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Data
{
    public interface IEntityDomainTemplate<TDto> : IEntityDomainTemplate<TDto, TDto, TDto, TDto>
        where TDto : class, IEntityDTO
    { }

    public interface IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> : IDomainTemplate
        where TListDTO : class, IEntityDTO
        where TCreateDTO : class, IEntityDTO
        where TEditDTO : class, IEntityDTO
        where TRemoveDTO : class
    {
        Task<IViewModel<TListDTO>> List();

        Task<IViewModel<TListDTO>> List(int page, int size);

        Task<IUpdateModel<TCreateDTO>> Create(TCreateDTO dto);

        Task<IUpdateModel<TEditDTO>> Edit(TEditDTO dto);

        Task Remove(TRemoveDTO dto);
    }
}
