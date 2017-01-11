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
        private Dictionary<string, List<SignOnTicketValue>> _Values;

        public SignOnTicket()
        {
            _Values = new Dictionary<string, List<SignOnTicketValue>>();
        }

        public string Identifier { get; set; }

        public string Name { get; set; }

        public byte[] Signature { get; set; }

        public void Add(SignOnTicketValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (_Values.ContainsKey(value.Name))
                _Values[value.Name].Add(value);
            else
                _Values.Add(value.Name, new List<SignOnTicketValue>() { value });
        }

        public void Remove(SignOnTicketValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            List<SignOnTicketValue> values;
            if (!_Values.TryGetValue(value.Name, out values))
                return;
            values.Remove(value);
            if (values.Count == 0)
                _Values.Remove(value.Name);
        }

        public SignOnTicketValue GetValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            return GetValues(name).FirstOrDefault();
        }

        public IEnumerable<SignOnTicketValue> GetValues(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            List<SignOnTicketValue> values;
            if (!_Values.TryGetValue(name, out values))
                return new SignOnTicketValue[0];
            return values;
        }
    }
}