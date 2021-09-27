using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data
{
    public class EntityEditMapEventArgs<TEntity, TDto> : DomainServiceEventArgs
    {
        public EntityEditMapEventArgs(TEntity entity, TDto dto)
        {
            Entity = entity;
            Dto = dto;
        }

        public TEntity Entity { get; }

        public TDto Dto { get; }
    }
}
