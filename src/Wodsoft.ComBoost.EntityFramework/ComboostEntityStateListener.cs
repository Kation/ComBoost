using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class ComboostEntityStateListener : IEntityStateListener
    {
        public ComboostEntityStateListener()
        {

        }

        public void StateChanged(InternalEntityEntry entry, EntityState oldState, bool skipInitialFixup, bool fromQuery)
        {
            
        }

        public void StateChanging(InternalEntityEntry entry, EntityState newState)
        {

        }
    }
}
