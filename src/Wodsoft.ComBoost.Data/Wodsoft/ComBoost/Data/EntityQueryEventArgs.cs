using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data
{
    public class EntityQueryEventArgs<T> : EventArgs
    {
        public EntityQueryEventArgs(IQueryable<T> queryable)
        {
            OriginQueryable = Queryable = queryable;
        }

        public IQueryable<T> OriginQueryable { get; private set; }

        public IQueryable<T> Queryable { get; set; }
    }
}
