using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;

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
        public JsonDocument Root { get; private set; }

        private Dictionary<string, string> _Values;
        protected override string[] GetKeysCore()
        {
            if (_Values == null)
            {
                try
                {
                    var reader = new StreamReader(HttpContext.Request.Body);
                    var text = reader.ReadToEnd();
                    Root = JsonDocument.Parse(text);
                }
                catch (Exception ex)
                {
                    throw new FormatException("解析Json内容失败。", ex);
                }
                _Values = new Dictionary<string, string>();
                GetValues(Root.RootElement.EnumerateObject(), _Values, "");
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

        private void GetValues(JsonElement.ObjectEnumerator children, Dictionary<string, string> values, string path)
        {
            foreach (var token in children)
            {
                var p = path + "." + token.Name;
                if (token.Value.ValueKind == JsonValueKind.Array)
                    GetValues(token.Value.EnumerateArray(), values, p);
                else if (token.Value.ValueKind == JsonValueKind.Object)
                    GetValues(token.Value.EnumerateObject(), values, p + ".");
                else
                {
                    values.Add(p, token.Value.GetRawText());
                }
            }
        }

        private void GetValues(JsonElement.ArrayEnumerator children, Dictionary<string, string> values, string path)
        {
            int i = 0;
            foreach (var token in children)
            {
                var p = path + "[" + i + "]";
                if (token.ValueKind == JsonValueKind.Array)
                    GetValues(token.EnumerateArray(), values, p);
                else if (token.ValueKind == JsonValueKind.Object)
                    GetValues(token.EnumerateObject(), values, p + ".");
                else
                {
                    values.Add(p, token.GetRawText());
                }
                i++;
            }
        }
    }
}
