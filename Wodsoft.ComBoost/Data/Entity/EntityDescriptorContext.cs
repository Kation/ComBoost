using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    /// <summary>
    /// Entity descriptor context.
    /// </summary>
    public class EntityDescriptorContext : ITypeDescriptorContext
    {
        private IEntityContextBuilder _Builder;

        /// <summary>
        /// Initialize entity descriptor context.
        /// </summary>
        /// <param name="builder">Entity context builder.</param>
        public EntityDescriptorContext(IEntityContextBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            _Builder = builder;
        }

        /// <summary>
        /// Get the container.
        /// </summary>
        public IContainer Container
        {
            get { return null; }
        }

        /// <summary>
        /// Get the entity context builder.
        /// </summary>
        public object Instance
        {
            get { return _Builder; }
        }

        /// <summary>
        /// Call when context changed.
        /// </summary>
        public void OnComponentChanged()
        {

        }

        /// <summary>
        /// Call when context changing.
        /// </summary>
        /// <returns></returns>
        public bool OnComponentChanging()
        {
            return false;
        }

        /// <summary>
        /// Get the property descriptor.
        /// </summary>
        public PropertyDescriptor PropertyDescriptor
        {
            get { return null; }
        }

        /// <summary>
        /// Get entity context.
        /// </summary>
        /// <param name="serviceType">Type of entity.</param>
        /// <returns>Return IEntityQueryable of entity.</returns>
        public object GetService(Type serviceType)
        {
            if (serviceType.IsGenericType)
            {
                Type definition = serviceType.GetGenericTypeDefinition();
                if (definition == typeof(IEntityQueryable<>))
                    return _Builder.GetContext(serviceType.GetGenericArguments()[0]);
                else
                    return null;
            }
            else
            {
                if (typeof(IEntity).IsAssignableFrom(serviceType))
                    return _Builder.GetContext(serviceType);
                else
                    return null;
            }
        }
    }
}
