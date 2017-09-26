using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainServiceAccessor : IDomainServiceAccessor
    {
        System.Threading.AsyncLocal<IDomainService> _Context = new System.Threading.AsyncLocal<IDomainService>();
        public IDomainService DomainService
        {
            get { return _Context.Value; }
            set { _Context.Value = value; }
        }
    }
}
