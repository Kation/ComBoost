using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Data
{
    public class EntityModelCreatedEventArgs<T> : DomainServiceEventArgs
        where T : IEntity
    {
        public EntityModelCreatedEventArgs(EntityEditModel<T> model)
        {
            Model = model;
        }

        public EntityEditModel<T> Model { get; set; }
    }
}
