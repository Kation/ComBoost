using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class CurrentDatabaseContext
    {
        public CurrentDatabaseContext(IServiceProvider privoder)
        {
            _Provider = privoder;
        }

        private IServiceProvider _Provider;

        public DatabaseContext Context
        {
            get;
            set;
        }
    }
}
