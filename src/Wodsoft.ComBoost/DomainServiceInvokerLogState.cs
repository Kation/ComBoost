using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class DomainServiceInvokerLogState : IEnumerable<KeyValuePair<string, object>>
    {
        public DomainServiceInvokerLogState(Type service, MethodInfo? method)
        {
            Service = service;
            Method = method;
        }

        public Type Service { get; }

        public MethodInfo? Method { get; }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            yield return new KeyValuePair<string, object>("DomainService", Service);
            yield return new KeyValuePair<string, object>("DomainMethod", Method);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
