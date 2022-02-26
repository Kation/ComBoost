using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class EntityDtoContextOptions<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>
        where TEntity : class, IEntity
        where TListDTO : class
        where TCreateDTO : class
        where TEditDTO : class
        where TRemoveDTO : class
    {
        public Action<TEntity, TCreateDTO>? OnAddMapped { get; set; }

        public Action<TEntity, TCreateDTO>? OnAdded { get; set; }

        public Action<TEntity, TEditDTO>? OnPreUpdateMap { get; set; }

        public Action<TEntity, TEditDTO>? OnUpdateMapped { get; set; }

        public Action<TEntity, TEditDTO>? OnUpdated { get; set; }

        public Action<TEntity, TRemoveDTO>? OnPreRemove { get; set; }

        public Action<TEntity, TRemoveDTO>? OnRemoved { get; set; }
    }
}
