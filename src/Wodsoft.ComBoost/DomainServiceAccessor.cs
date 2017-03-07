using System;
using System.Collections.Generic;
using System.Linq;
#if NET451
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#endif
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainServiceAccessor : IDomainServiceAccessor
    {
#if NET451
        private static readonly string LogicalDataKey = "__DomainService_Current__" + AppDomain.CurrentDomain.Id;

        public IDomainService DomainService
        {
            get
            {
                var handle = CallContext.LogicalGetData(LogicalDataKey) as ObjectHandle;
                return handle?.Unwrap() as IDomainService;
            }
            set
            {
                CallContext.LogicalSetData(LogicalDataKey, new ObjectHandle(value));
            }
        }
#else
        System.Threading.AsyncLocal<IDomainService> _Context = new System.Threading.AsyncLocal<IDomainService>();
        public IDomainService DomainService
        {
            get { return _Context.Value; }
            set { _Context.Value = value; }
        }
#endif
    }
}
