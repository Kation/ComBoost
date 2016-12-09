using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if NET451
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#endif

namespace Wodsoft.ComBoost.Data.Entity
{
    public class CurrentDatabaseContext
    {
#if NET451        
        private static readonly string LogicalDataKey = "__DatabaseContext_Current__" + AppDomain.CurrentDomain.Id;

        public DatabaseContext Context
        {
            get
            {
                var handle = CallContext.LogicalGetData(LogicalDataKey) as ObjectHandle;
                return handle?.Unwrap() as DatabaseContext;
            }
            set
            {
                CallContext.LogicalSetData(LogicalDataKey, new ObjectHandle(value));
            }
        }
#else
        System.Threading.AsyncLocal<DatabaseContext> _Context = new System.Threading.AsyncLocal<DatabaseContext>();
        public DatabaseContext Context
        {
            get { return _Context.Value; }
            set { _Context.Value = value; }
        }
#endif
    }
}
