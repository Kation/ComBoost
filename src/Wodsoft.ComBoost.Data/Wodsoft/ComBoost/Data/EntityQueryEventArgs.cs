using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data
{
    public class EntityQueryEventArgs<T> : DomainServiceEventArgs
    {
        public EntityQueryEventArgs(IAsyncQueryable<T> queryable)
        {
            OriginQueryable = Queryable = queryable;
            IsOrdered = false;
        }

        public IAsyncQueryable<T> OriginQueryable { get; private set; }

        public IAsyncQueryable<T> Queryable { get; set; }

        public bool IsOrdered { get; set; }
    }
}
