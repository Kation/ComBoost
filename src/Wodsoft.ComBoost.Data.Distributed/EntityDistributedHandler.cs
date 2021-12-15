using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Distributed
{
    public class EntityDistributedHandler<TEntity, TDto> : IDomainServiceEventHandler<EntityEditedEventArgs<TEntity>>, IDomainServiceEventHandler<EntityRemovedEventArgs<TEntity>>
        where TEntity : class, IEntity
        where TDto : class
    {
        public Task Handle(IDomainExecutionContext context, EntityEditedEventArgs<TEntity> args)
        {
            return RaiseEvent(context, args.Entity);
        }

        public Task Handle(IDomainExecutionContext context, EntityRemovedEventArgs<TEntity> args)
        {
            return RaiseEvent(context, args.Entity);
        }

        private Task RaiseEvent(IDomainExecutionContext context, TEntity entity)
        {
            var mapper = context.DomainContext.GetRequiredService<IMapper>();
            var dto = mapper.Map<TDto>(entity);
            var descriptor = EntityDescriptor.GetMetadata<TDto>();
            var e = new ObjectChangedEventArgs<TDto> { Keys = descriptor.KeyProperties.Select(t => t.GetValue(dto).ToString()).ToArray() };
            return context.DomainContext.EventManager.RaiseEvent(context, e);
        }
    }
}
