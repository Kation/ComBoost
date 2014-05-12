using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Routing
{
    public class EntityRoute : Route
    {
        public EntityRoute(string url, IRouteHandler routeHandler) : base(url, routeHandler) { }

        private Type _UserType;
        public Type UserType
        {
            get { return _UserType; }
            set
            {
                if (!typeof(IRoleEntity).IsAssignableFrom(value))
                    throw new ArgumentException("value doesn't inherit IRoleEntity.");
                _UserType = value;
            }
        }
    }
}
