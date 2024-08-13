using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ValueKeyCollection : IValueKeyCollection
    {
        private List<string> originKeys;

        public ValueKeyCollection(IEnumerable<string> keys)
        {
            originKeys = keys.ToList();
        }

        public int Count => originKeys.Count;

        public virtual bool ContainsKey(string key)
        {
            return originKeys.Contains(key);
        }

        public virtual bool ContainsPrefix(string prefix, params char[] separators)
        {
            return originKeys.Any(t => t == prefix || separators.Any(x => t.StartsWith(prefix + x)));
        }

        public IEnumerator<string> GetEnumerator()
        {
            return originKeys.GetEnumerator();
        }

        public virtual IDictionary<string, string> GetKeysFromPrefix(string prefix, params char[] separators)
        {
            var data = originKeys.Where(t => separators.Any(x => t.StartsWith(prefix + x))).ToDictionary(t =>
            {
                var text = t.Substring(prefix.Length);
                int i = text.IndexOfAny(separators, 1);
                if (i == -1)
                    return text.Substring(1);
                return text.Substring(1, i - 1);
            }, t =>
            {
                var text = t.Substring(prefix.Length);
                int i = text.IndexOfAny(separators, 1);
                if (i == -1)
                    return prefix + text;
                return prefix + text.Substring(0, i);
            });
            return data;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return originKeys.GetEnumerator();
        }
    }
}
