using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public static class ControllerExtensions
    {
        public static EntityControllerUnitils<T> GetUntitils<T>(this Controller controller) where T : class, IEntity, new()
        {
            var builder = controller.Resolver.GetService<IEntityContextBuilder>();
            if (builder == null)
                throw new NotSupportedException("Can not resolve IEntityContextBuilder.");
            return new EntityControllerUnitils<T>(controller, builder);
        }
    }
}
