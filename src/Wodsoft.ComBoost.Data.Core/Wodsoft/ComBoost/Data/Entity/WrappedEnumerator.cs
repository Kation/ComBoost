using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class WrappedEnumerator<T, M> : IEnumerator<T>
            where T : IEntity
            where M : IEntity, T
    {
        public WrappedEnumerator(IEnumerator<M> enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));
            InnerEnumerator = enumerator;
        }

        public IEnumerator<M> InnerEnumerator { get; private set; }

        public T Current { get { return InnerEnumerator.Current; } }

        object IEnumerator.Current { get { return InnerEnumerator.Current; } }
        public void Dispose()
        {
            InnerEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            return InnerEnumerator.MoveNext();
        }

        public void Reset()
        {
            InnerEnumerator.Reset();
        }
    }
}
