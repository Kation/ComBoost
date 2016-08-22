using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data
{
    public class EntityModelCreatedEventArgs<T> : EventArgs
    {
        public EntityModelCreatedEventArgs(EntityEditModel<T> model)
        {
            Model = model;
        }

        public EntityEditModel<T> Model { get; set; }
    }
}
