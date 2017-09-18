using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Wodsoft.ComBoost.Data.Entity
{
    public static class DatabaseContextAccessor
    {
        public static IDatabaseContext Context
        {
            get { return RequestScope.Current.Get<IDatabaseContext>("__IDatabaseContext"); }
            set
            {
                RequestScope.Current["__IDatabaseContext"] = value;
            }
        }
    }
}