using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data
{
    public class EntityPreMapEventArgs<TEntity, TDto> : DomainServiceEventArgs
    {
        public EntityPreMapEventArgs(TEntity entity, TDto dto)
        {
            Entity = entity;
            Dto = dto;
        }

        public TEntity Entity { get; }

        public TDto Dto { get; }
    }
}
