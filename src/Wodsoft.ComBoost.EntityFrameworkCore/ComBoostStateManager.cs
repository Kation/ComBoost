using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class ComBoostStateManager : StateManager
    {
        public ComBoostStateManager(
            IInternalEntityEntryFactory factory,
            IInternalEntityEntrySubscriber subscriber,
            IInternalEntityEntryNotifier notifier,
            IValueGenerationManager valueGeneration,
            IModel model,
            IDatabase database,
            IConcurrencyDetector concurrencyDetector,
            ICurrentDbContext currentContext,
            CurrentDatabaseContext currentDatabase)
            : base(factory, subscriber, notifier, valueGeneration, model, database, concurrencyDetector, currentContext)
        {
            _CurrentDatabase = currentDatabase;
        }

        private CurrentDatabaseContext _CurrentDatabase;

        public override InternalEntityEntry StartTrackingFromQuery(IEntityType baseEntityType, object entity, ValueBuffer valueBuffer, ISet<IForeignKey> handledForeignKeys)
        {
            if (entity is IEntity)
            {
                var context = _CurrentDatabase.Context.GetDynamicContext(baseEntityType.ClrType);
                ((IEntity)entity).EntityContext = context;
                return null;
            }
            return base.StartTrackingFromQuery(baseEntityType, entity, valueBuffer, handledForeignKeys);
        }
    }
}
