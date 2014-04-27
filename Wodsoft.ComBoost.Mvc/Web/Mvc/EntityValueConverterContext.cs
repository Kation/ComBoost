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
    public class EntityValueConverterContext : ITypeDescriptorContext
    {
        private EntityDescriptorContext _Context;

        public EntityValueConverterContext(EntityDescriptorContext context, PropertyMetadata property)
        {
            _Context = context;
            Property = property;
        }

        public PropertyMetadata Property { get; private set; }

        public IContainer Container
        {
            get { return _Context.Container; }
        }

        public object Instance
        {
            get { return _Context.Instance; }
        }

        public void OnComponentChanged()
        {
            _Context.OnComponentChanged();
        }

        public bool OnComponentChanging()
        {
            return _Context.OnComponentChanging();
        }

        public PropertyDescriptor PropertyDescriptor
        {
            get { return _Context.PropertyDescriptor; }
        }

        public object GetService(Type serviceType)
        {
            return _Context.GetService(serviceType);
        }
    }
}
