using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// Controller extensions.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Get untitils for controller.
        /// </summary>
        /// <typeparam name="T">Type of entity.</typeparam>
        /// <param name="controller">Controller.</param>
        /// <returns></returns>
        public static EntityControllerUnitils<T> GetUntitils<T>(this Controller controller) where T : class, IEntity, new()
        {
            if (controller == null)
                throw new ArgumentNullException("controller");
            object unitils;
            if (!controller.TempData.TryGetValue("controllerUntitils", out unitils))
            {
                var builder = controller.Resolver.GetService<IEntityContextBuilder>();
                if (builder == null)
                    throw new NotSupportedException("Can not resolve IEntityContextBuilder.");
                unitils = new EntityControllerUnitils<T>(controller, builder);
                controller.TempData.Add("controllerUntitils", unitils);
            }
            return (EntityControllerUnitils<T>)unitils;
        }
    }
}
