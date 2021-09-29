using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Wodsoft.ComBoost.Data
{
    public class EntityQueryModelCreatedEventArgs<TModel> : DomainServiceEventArgs
    {
        public EntityQueryModelCreatedEventArgs(IViewModel<TModel> model)
        {
            Model = model;
        }

        public IViewModel Model { get; }
    }
}
