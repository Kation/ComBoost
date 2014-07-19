using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization.Formatters
{
    internal class SerializerHeader<T>
    {
        private List<T> _Collection;

        public SerializerHeader()
        {
            _Collection = new List<T>();
        }

        public int GetIndex(T header)
        {
            if (!_Collection.Contains(header))
                _Collection.Add(header);
            return _Collection.IndexOf(header);
        }

        public T GetHeader(int index)
        {
            return _Collection[index];
        }

        public void AddHeader(T header)
        {
            _Collection.Add(header);
        }

        public int Count { get { return _Collection.Count; } }
    }
}
