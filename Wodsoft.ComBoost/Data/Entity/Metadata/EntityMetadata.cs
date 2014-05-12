using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Entity.Metadata
{
    /// <summary>
    /// Entity metadata.
    /// </summary>
    public class EntityMetadata
    {
        private Dictionary<string, PropertyMetadata> _Properties;

        /// <summary>
        /// Initialize entity metadata.
        /// </summary>
        /// <param name="type"></param>
        public EntityMetadata(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Type = type;
            KeyType = type.GetProperty("Index").PropertyType;

            PropertyInfo[] properties = type.GetProperties().Where(t => t.GetGetMethod() != null && t.GetSetMethod() != null).ToArray();
            Properties = properties.Select(t => new PropertyMetadata(t)).OrderBy(t => t.Order).ToArray();

            ViewProperties = Properties.Where(t => !t.IsHiddenOnView).ToArray();
            EditProperties = Properties.Where(t => !t.IsHiddenOnEdit).ToArray();
            SearchProperties = Properties.Where(t => t.Searchable).ToArray();
            DetailProperties = Properties.Where(t => !t.IsHiddenOnView || !t.IsHiddenOnEdit).ToArray();

            _Properties = new Dictionary<string, PropertyMetadata>();
            for (int i = 0; i < Properties.Length; i++)
                _Properties.Add(Properties[i].Property.Name, Properties[i]);

            EntityAuthenticationAttribute authenticate = type.GetCustomAttribute<EntityAuthenticationAttribute>();
            if (authenticate == null)
            {
                AllowAnonymous = true;
                AddRoles = new string[0];
                EditRoles = new string[0];
                ViewRoles = new string[0];
                RemoveRoles = new string[0];
            }
            else
            {
                AllowAnonymous = authenticate.AllowAnonymous;
                AddRoles = authenticate.AddRolesRequired;
                EditRoles = authenticate.EditRolesRequired;
                ViewRoles = authenticate.ViewRolesRequired;
                RemoveRoles = authenticate.RemoveRolesRequired;
            }

            DisplayNameAttribute display = type.GetCustomAttribute<DisplayNameAttribute>();
            if (display != null)
                Name = display.DisplayName == null ? type.Name : display.DisplayName;
            else
                Name = type.Name;

            DisplayColumnAttribute displayColumn = type.GetCustomAttribute<DisplayColumnAttribute>();
            if (displayColumn != null)
            {
                DisplayProperty = GetProperty(displayColumn.DisplayColumn);
                if (displayColumn.SortColumn != null)
                {
                    SortProperty = Properties.SingleOrDefault(t => t.Property.Name == displayColumn.SortColumn);
                    SortDescending = displayColumn.SortDescending;
                }
            }
            else
                DisplayProperty = GetProperty("Index");
            ParentAttribute parent = type.GetCustomAttribute<ParentAttribute>();
            if (parent != null)
                ParentProperty = Properties.SingleOrDefault(t => t.Property.Name == parent.PropertyName);
        }

        /// <summary>
        /// Get the system type of entity.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Get the system type of key of entity.
        /// </summary>
        public Type KeyType { get; private set; }

        /// <summary>
        /// Get the display name of entity.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Get the display property of entity.
        /// </summary>
        public PropertyMetadata DisplayProperty { get; private set; }

        /// <summary>
        /// Get the sort property of entity.
        /// </summary>
        public PropertyMetadata SortProperty { get; private set; }

        /// <summary>
        /// Get the parent property of entity.
        /// </summary>
        public PropertyMetadata ParentProperty { get; private set; }

        /// <summary>
        /// Get the sort mode of entity.
        /// </summary>
        public bool SortDescending { get; private set; }

        /// <summary>
        /// Get the properties of entity.
        /// </summary>
        public PropertyMetadata[] Properties { get; private set; }

        /// <summary>
        /// Get the properties of entity in viewlist.
        /// </summary>
        public PropertyMetadata[] ViewProperties { get; private set; }

        /// <summary>
        /// Get the properties of entity while edit.
        /// </summary>
        public PropertyMetadata[] EditProperties { get; private set; }

        /// <summary>
        /// Get the properties of entity while search.
        /// </summary>
        public PropertyMetadata[] SearchProperties { get; private set; }
        
        /// <summary>
        /// Get the properties of entity in detail.
        /// </summary>
        public PropertyMetadata[] DetailProperties { get; private set; }

        /// <summary>
        /// Get is entity allow anonymous operate.
        /// </summary>
        public bool AllowAnonymous { get; private set; }

        /// <summary>
        /// Get the roles to view entity.
        /// </summary>
        public string[] ViewRoles { get; private set; }

        /// <summary>
        /// Get the roles to add entity.
        /// </summary>
        public string[] AddRoles { get; private set; }

        /// <summary>
        /// Get the roles to edit entity.
        /// </summary>
        public string[] EditRoles { get; private set; }

        /// <summary>
        /// Get the roles to remove entity.
        /// </summary>
        public string[] RemoveRoles { get; private set; }

        /// <summary>
        /// Get the property of entity.
        /// </summary>
        /// <param name="name">Name of property.</param>
        /// <returns>Return property metadata. Return null if property doesn't exists.</returns>
        public PropertyMetadata GetProperty(string name)
        {
            if (!_Properties.ContainsKey(name))
                return null;
            return _Properties[name];
        }
    }
}
