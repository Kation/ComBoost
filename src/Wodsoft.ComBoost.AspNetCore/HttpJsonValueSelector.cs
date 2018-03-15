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
    /// <summary>
    /// HttpJson值选择器。
    /// </summary>
    public class HttpJsonValueSelector : HttpValueSelector
    {
        /// <summary>
        /// 实例化选择器。
        /// </summary>
        /// <param name="httpContext">Http上下文。</param>
        public HttpJsonValueSelector(HttpContext httpContext) : base(httpContext)
        {
        }

        /// <summary>
        /// 获取Json根。
        /// </summary>
        public JContainer Root { get; private set; }

        private Dictionary<string, string> _Values;
        protected override string[] GetKeysCore()
        {
            if (_Values == null)
            {
                try
                {
                    var reader = new StreamReader(HttpContext.Request.Body);
                    var text = reader.ReadToEnd();
                    Root = JsonConvert.DeserializeObject<JContainer>(text);
                }
                catch (Exception ex)
                {
                    throw new FormatException("解析Json内容失败。", ex);
                }
                _Values = new Dictionary<string, string>();
                GetValues(Root.Children(), _Values);
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
