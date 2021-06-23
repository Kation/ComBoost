using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class DomainDistributedExecutionContext : IDomainExecutionContext
    {
        public DomainDistributedExecutionContext(IDomainContext domainContext)
        {
            DomainContext = domainContext ?? throw new ArgumentNullException(nameof(domainContext));
        }

        public IDomainContext DomainContext { get; }

        public IDomainService DomainService => null;

        public MethodInfo DomainMethod => null;

        public object[] ParameterValues => null;

        public object Result => null;

        public bool IsAborted => false;

        public bool IsCompleted => false;

        public void Done()
        {
            throw new NotSupportedException();
        }

        public void Done(object result)
        {
            throw new NotSupportedException();
        }

        public object GetParameterValue(ParameterInfo parameter)
        {
            throw new NotSupportedException();
        }
    }
}
