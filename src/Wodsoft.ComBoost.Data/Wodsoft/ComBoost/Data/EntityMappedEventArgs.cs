using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data
{
    public class EntityMappedEventArgs<TEntity, TDto> : DomainServiceEventArgs
    {
        public EntityMappedEventArgs(TEntity entity, TDto dto)
        {
            Entity = entity;
            Dto = dto;
        }

        public TEntity Entity { get; }

        public TDto Dto { get; }
    }
}
