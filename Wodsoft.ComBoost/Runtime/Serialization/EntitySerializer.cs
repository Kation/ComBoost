using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Comboost entity serializer.
    /// </summary>
    public class EntitySerializer : ComBoostSerializer
    {
        /// <summary>
        /// Serialize value.
        /// </summary>
        /// <param name="stream">Data stream.</param>
        /// <param name="type">Type of value.</param>
        /// <param name="value">Value to serialize.</param>
        protected override void SerializeValue(IO.Stream stream, Type type, object value)
        {
            if (typeof(IEntity).IsAssignableFrom(type))
            {
                base.SerializeValue(stream, typeof(Guid), ((IEntity)value).Index);
                return;
            }
            base.SerializeValue(stream, type, value);
        }
    }
}
