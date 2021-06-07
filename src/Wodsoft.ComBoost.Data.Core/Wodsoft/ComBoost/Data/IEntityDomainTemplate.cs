using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Data
{
    public interface IEntityDomainTemplate<TKey,TDto> : IEntityDomainTemplate<TKey, TDto, TDto, TDto>
        where TDto : class, IEntityDTO<TKey>
    { }

    public interface IEntityDomainTemplate<TKey, TListDTO, TCreateDTO, TEditDTO> : IDomainTemplate
        where TListDTO : class, IEntityDTO<TKey>
        where TCreateDTO : class, IEntityDTO<TKey>
        where TEditDTO : class, IEntityDTO<TKey>
    {
        Task<IViewModel<TListDTO>> List();

        Task<IViewModel<TListDTO>> List(int page, int size);

        Task<IUpdateModel<TCreateDTO>> Create(TCreateDTO dto);

        Task<IUpdateModel<TEditDTO>> Edit(TEditDTO dto);

        Task<IUpdateModel> Remove(TKey id);
    }
}
