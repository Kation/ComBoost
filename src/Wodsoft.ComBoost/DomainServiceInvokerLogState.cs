using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class DomainServiceInvokerLogState
    {
        public DomainServiceInvokerLogState(Type service, MethodInfo? method)
        {
            Service = service;
            Method = method;
        }

        public Type Service { get; }

        public MethodInfo? Method { get; }
    }
}
