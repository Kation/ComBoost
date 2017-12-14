using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class HttpJsonValueSelector : HttpValueSelector
    {
        public HttpJsonValueSelector(HttpContext httpContext) : base(httpContext)
        {
        }

        private Dictionary<string, string> _Values;
        protected override string[] GetKeysCore()
        {
            if (_Values == null)
            {
                JContainer jobj;
                try
                {
                    var reader = new StreamReader(HttpContext.Request.Body);
                    var text = reader.ReadToEnd();
                    jobj = JsonConvert.DeserializeObject<JContainer>(text);
                }
                catch (Exception ex)
                {
                    throw new FormatException("解析Json内容失败。", ex);
                }
                _Values = new Dictionary<string, string>();
                GetValues(jobj.Children(), _Values);
            }
            return _Values.Keys.ToArray();
        }

        protected override object GetValueCore(string key)
        {
            string value;
            if (_Values.TryGetValue(key, out value))
                return value;
            return null;
        }

        private void GetValues(JEnumerable<JToken> children, Dictionary<string, string> values)
        {
            foreach (var token in children)
            {
                if (token.HasValues)
                    GetValues(token.Children(), values);
                else
                    if (token.Type == JTokenType.Object || token.Type == JTokenType.Array)
                    values.Add(token.Path, null);
                else
                    values.Add(token.Path, token.ToObject<string>());
            }
        }
    }
}
