using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.SingleSignOn
{
    public class SignOnTicket
    {
        private Dictionary<string, string> _Data;

        public SignOnTicket()
        {
            _Data = new Dictionary<string, string>();
        }

        protected string GetValue(string key)
        {
            string value;
            if (!_Data.TryGetValue(key, out value))
                return null;
            return value;
        }

        protected void SetValue(string key, string value)
        {
            if (_Data.ContainsKey(key))
                _Data[key] = value;
            else
                _Data.Add(key, value);
        }

        public virtual byte[] GetData()
        {
            var data = JsonConvert.SerializeObject(_Data);
            return Encoding.UTF8.GetBytes(data);
        }

        public virtual void SetData(byte[] data)
        {
            try
            {
                var json = Encoding.UTF8.GetString(data);
                _Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch
            {
                throw new FormatException("传入的数据格式不正确。");
            }
        }

        public virtual IDictionary<string, string> GetValues()
        {
            return _Data;
        }
    }
}
