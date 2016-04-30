using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public class ReadableStringProvider : IValueProvider
    {
        public ReadableStringProvider(IReadableStringCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            Collection = collection;
        }

        public IReadableStringCollection Collection { get; private set; }

        public object GetValue(string name)
        {
            return Collection[name];
        }
    }
}
