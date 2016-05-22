using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IEntityMappedContext<T>
    {
        void Add(T item);
        T Create();
        Task<T> GetAsync(object key);

    }
}
