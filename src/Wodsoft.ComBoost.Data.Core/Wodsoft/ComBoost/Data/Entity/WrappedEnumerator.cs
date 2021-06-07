using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class WrappedEnumerator<T, M> : IAsyncEnumerator<T>
            where T : IEntity
            where M : IEntity, T
    {
        public WrappedEnumerator(IAsyncEnumerator<M> enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));
            InnerEnumerator = enumerator;
        }

        public IAsyncEnumerator<M> InnerEnumerator { get; private set; }

        public T Current => InnerEnumerator.Current;

        public ValueTask DisposeAsync()
        {
            return InnerEnumerator.DisposeAsync();
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return InnerEnumerator.MoveNextAsync();
        }
    }
}
