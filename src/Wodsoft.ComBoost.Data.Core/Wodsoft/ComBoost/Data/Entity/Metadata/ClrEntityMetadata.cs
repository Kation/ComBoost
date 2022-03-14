using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity.Metadata
{
    /// <summary>
    /// Entity metadata.
    /// </summary>
    public class ClrEntityMetadata : EntityMetadataBase
    {
        /// <summary>
        /// Initialize entity metadata.
        /// </summary>
        /// <param name="type"></param>
        public ClrEntityMetadata(Type type)
            : base(type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            _properties = type.GetProperties().Select(t => new ClrPropertyMetadata(t)).OrderBy(t => t.Order).ToDictionary(t => t.ClrName, t => t);
            Properties = new ReadOnlyCollection<IPropertyMetadata>(_properties.Values.ToArray());

            DisplayNameAttribute display = type.GetCustomAttribute<DisplayNameAttribute>();
            if (display != null)
                Name = display.DisplayName;
            else
                Name = Type.Name;

            ParentAttribute parent = Type.GetTypeInfo().GetCustomAttribute<ParentAttribute>();
            if (parent != null)
                ParentProperty = GetProperty(parent.PropertyName) ?? throw new InvalidOperationException($"Type \"{Type.FullName}\" does not contains parent property \"{parent.PropertyName}\".");

            var keys = Properties.Where(t => t.IsKey).ToArray();
            if (keys.Length == 0)
            {
                var multipleKeyAttribute = type.GetCustomAttribute<MultipleKeyAttribute>();
                if (multipleKeyAttribute == null && multipleKeyAttribute.Keys.Length != 0)
                {
                    var key = GetProperty("Id") ?? GetProperty("ID") ?? GetProperty("Index") ?? GetProperty(Type.Name + "Id") ?? GetProperty(Type.Name + "ID");
                    if (key != null)
                        KeyProperties = new ReadOnlyCollection<IPropertyMetadata>(new IPropertyMetadata[] { key });
                    else
                        throw new NotSupportedException($"Type \"{Type.FullName}\" does not contains key property.");
                }
                else
                {
                    KeyProperties = new ReadOnlyCollection<IPropertyMetadata>(multipleKeyAttribute.Keys.Select(t => GetProperty(t) ?? throw new InvalidOperationException($"Type \"{Type.FullName}\" does not contains key property.")).ToArray());
                }
            }
            else
            {
                KeyProperties = new ReadOnlyCollection<IPropertyMetadata>(keys);
            }

            DisplayColumnAttribute displayColumn = Type.GetTypeInfo().GetCustomAttribute<DisplayColumnAttribute>();
            if (displayColumn != null)
            {
                DisplayProperty = GetProperty(displayColumn.DisplayColumn);
                if (displayColumn.SortColumn != null)
                    SortProperty = GetProperty(displayColumn.SortColumn) ?? throw new InvalidOperationException($"Type \"{Type.FullName}\" does not contains sort property \"{displayColumn.SortColumn}\".");
                IsSortDescending = displayColumn.SortDescending;
            }

            if (SortProperty == null)
                SortProperty = KeyProperties[0];
        }

        private Dictionary<string, ClrPropertyMetadata> _properties;
        public override IReadOnlyList<IPropertyMetadata> KeyProperties { get; }

        public override string Name { get; }

        public override IPropertyMetadata SortProperty { get; }

        public override IReadOnlyList<IPropertyMetadata> Properties { get; }

        public override IPropertyMetadata? ParentProperty { get; }

        public override IPropertyMetadata? GetProperty(string name)
        {
            ClrPropertyMetadata value;
            if (_properties.TryGetValue(name, out value))
                return value;
            return null;
        }
    }
}
