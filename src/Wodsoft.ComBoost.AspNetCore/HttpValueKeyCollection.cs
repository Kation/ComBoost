using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class HttpValueKeyCollection : ValueKeyCollection
    {
        private Dictionary<string, string> lowerKeys;

        public HttpValueKeyCollection(IEnumerable<string> keys, bool ignoreCase) : base(keys)
        {
            if (ignoreCase)
            {
                lowerKeys = new Dictionary<string, string>();
                foreach (var key in keys)
                {
                    var lower = key.ToLower();
                    if (lowerKeys.ContainsKey(lower))
                        continue;
                    lowerKeys.Add(lower, key);
                }
            }
        }

        public override bool ContainsKey(string key)
        {
            if (lowerKeys != null)
                return lowerKeys.ContainsKey(key.ToLower());
            return base.ContainsKey(key);
        }

        public override bool ContainsPrefix(string prefix, params char[] separators)
        {
            if (lowerKeys != null)
            {
                prefix = prefix.ToLower();
                return lowerKeys.Keys.Any(t => t == prefix || separators.Any(x => t.StartsWith(prefix + x)));
            }
            return base.ContainsPrefix(prefix, separators);
        }

        public override IDictionary<string, string> GetKeysFromPrefix(string prefix, params char[] separators)
        {
            if (lowerKeys != null)
            {
                prefix = prefix.ToLower();
                var data = lowerKeys.Where(t => separators.Any(x => t.Key.StartsWith(prefix + x))).ToDictionary(t =>
                {
                    var text = t.Value.Substring(prefix.Length);
                    int i = text.IndexOfAny(separators, 1);
                    if (i == -1)
                        return text.Substring(1);
                    return text.Substring(1, i - 1);
                }, t =>
                {
                    var text = t.Value.Substring(prefix.Length);
                    int i = text.IndexOfAny(separators, 1);
                    if (i == -1)
                        return t.Value;
                    return t.Value.Substring(0, prefix.Length) + text.Substring(0, i);
                });
                return data;
            }
            return base.GetKeysFromPrefix(prefix, separators);
        }
    }
}
