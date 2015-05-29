using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// Entity value converter type descriptor context.
    /// </summary>
    public class EntityValueConverterContext : ITypeDescriptorContext
    {
        private EntityDescriptorContext _Context;

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">Entity descriptor context.</param>
        /// <param name="property">Property metadata.</param>
        public EntityValueConverterContext(EntityDescriptorContext context, IPropertyMetadata property)
        {
            _Context = context;
            Property = property;
        }

        /// <summary>
        /// Get the property convert to.
        /// </summary>
        public IPropertyMetadata Property { get; private set; }

        /// <summary>
        /// Get the container.
        /// </summary>
        public IContainer Container
        {
            get { return _Context.Container; }
        }

        /// <summary>
        /// Get the entity context builder.
        /// </summary>
        public object Instance
        {
            get { return _Context.Instance; }
        }

        /// <summary>
        /// Raises the System.ComponentModel.Design.IComponentChangeService.ComponentChanged event.
        /// </summary>
        public void OnComponentChanged()
        {
            _Context.OnComponentChanged();
        }

        /// <summary>
        /// Raises the System.ComponentModel.Design.IComponentChangeService.ComponentChanging event.
        /// </summary>
        /// <returns></returns>
        public bool OnComponentChanging()
        {
            return _Context.OnComponentChanging();
        }
        /// <summary>
        /// Gets the System.ComponentModel.PropertyDescriptor that is associated with the given context item.
        /// </summary>
        public PropertyDescriptor PropertyDescriptor
        {
            get { return _Context.PropertyDescriptor; }
        }

        /// <summary>
        /// Get the entity context.
        /// </summary>
        /// <param name="serviceType">Type of entity.</param>
        /// <returns>IEntityQueryable&lt;&gt;</returns>
        public object GetService(Type serviceType)
        {
            return _Context.GetService(serviceType);
        }
    }
}
