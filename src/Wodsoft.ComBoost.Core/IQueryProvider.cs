using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IQueryProvider
    {
        bool HasQuery { get; }

        IQueryable<T> Query<T>(IQueryable<T> queryable);
    }
}
