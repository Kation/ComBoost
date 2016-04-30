using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace System.Data.Entity.Metadata
{
    /// <summary>
    /// Entity metadata base class.
    /// </summary>
    public abstract class EntityMetadataBase : IEntityMetadata
    {
        /// <summary>
        /// Initialize entity metadata.
        /// </summary>
        /// <param name="entityType"></param>
        protected EntityMetadataBase(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");
            Type = entityType;
            var key = entityType.GetProperties().FirstOrDefault(t => t.GetCustomAttribute<KeyAttribute>() != null);
            if (key != null)
                KeyType = key.PropertyType;
            else
            {
                key = entityType.GetProperty("Index");
                if (key != null)
                    KeyType = key.PropertyType;
                else
                {
                    key = entityType.GetProperty("Id");
                    if (key != null)
                        KeyType = key.PropertyType;
                    else
                    {
                        key = entityType.GetProperty("ID");
                        if (key != null)
                            KeyType = key.PropertyType;
                    }
                }
            }
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
        public string Name { get; protected set; }

        /// <summary>
        /// Get the display property of entity.
        /// </summary>
        public IPropertyMetadata DisplayProperty { get; protected set; }

        /// <summary>
        /// Get the sort property of entity.
        /// </summary>
        public IPropertyMetadata SortProperty { get; protected set; }

        /// <summary>
        /// Get the parent property of entity.
        /// </summary>
        public IPropertyMetadata ParentProperty { get; protected set; }

        /// <summary>
        /// Get the sort mode of entity.
        /// </summary>
        public bool SortDescending { get; protected set; }

        /// <summary>
        /// Get the properties of entity.
        /// </summary>
        public IEnumerable<IPropertyMetadata> Properties { get; private set; }

        /// <summary>
        /// Get the properties of entity in viewlist.
        /// </summary>
        public IEnumerable<IPropertyMetadata> ViewProperties { get; private set; }

        /// <summary>
        /// Get the properties of entity while create.
        /// </summary>
        public IEnumerable<IPropertyMetadata> CreateProperties { get; private set; }

        /// <summary>
        /// Get the properties of entity while edit.
        /// </summary>
        public IEnumerable<IPropertyMetadata> EditProperties { get; private set; }

        /// <summary>
        /// Get the properties of entity while search.
        /// </summary>
        public IEnumerable<IPropertyMetadata> SearchProperties { get; private set; }

        /// <summary>
        /// Get the properties of entity in detail.
        /// </summary>
        public IEnumerable<IPropertyMetadata> DetailProperties { get; private set; }

        /// <summary>
        /// Get is entity allow anonymous operate.
        /// </summary>
        public bool AllowAnonymous { get; protected set; }

        /// <summary>
        /// Get roles to view entity.
        /// </summary>
        public IEnumerable<object> ViewRoles { get; protected set; }

        /// <summary>
        /// Get roles to add entity.
        /// </summary>
        public IEnumerable<object> AddRoles { get; protected set; }

        /// <summary>
        /// Get roles to edit entity.
        /// </summary>
        public IEnumerable<object> EditRoles { get; protected set; }

        /// <summary>
        /// Get roles to remove entity.
        /// </summary>
        public IEnumerable<object> RemoveRoles { get; protected set; }

        /// <summary>
        /// Get the authentication required mode.
        /// </summary>
        public AuthenticationRequiredMode AuthenticationRequiredMode { get; protected set; }

        /// <summary>
        /// Get the cache of properties.
        /// </summary>
        protected ReadOnlyDictionary<string, IPropertyMetadata> PropertyCache { get; private set; }

        /// <summary>
        /// Get the property of entity.
        /// </summary>
        /// <param name="name">Name of property.</param>
        /// <returns>Return property metadata. Return null if property doesn't exists.</returns>
        public virtual IPropertyMetadata GetProperty(string name)
        {
            IPropertyMetadata value;
            if (PropertyCache.TryGetValue(name, out value))
                return value;
            return null;
        }

        /// <summary>
        /// Set the metadata of display.
        /// </summary>
        /// <param name="display">Display name attribute.</param>
        protected virtual void SetDisplay(DisplayNameAttribute display)
        {
            if (display == null)
                throw new ArgumentNullException("display");
            Name = display.DisplayName;
        }


        /// <summary>
        /// Set the metadata of display property.
        /// </summary>
        /// <param name="display">Display attribute.</param>
        protected virtual void SetDisplayColumn(DisplayColumnAttribute display)
        {
            if (display == null)
                throw new ArgumentNullException("display");
            DisplayProperty = GetProperty(display.DisplayColumn);
            if (display.SortColumn != null)
            {
                SortProperty = GetProperty(display.SortColumn);
                SortDescending = display.SortDescending;
            }
        }


        /// <summary>
        /// Set the metadata of parent.
        /// </summary>
        /// <param name="parent">Parent attribute.</param>
        protected virtual void SetParent(ParentAttribute parent)
        {
            if (parent == null)
                throw new ArgumentNullException("display");
            ParentProperty = GetProperty(parent.PropertyName);
        }

        /// <summary>
        /// Set the metadata of properties.
        /// </summary>
        /// <param name="propertyMetadatas">Property metadatas.</param>
        protected virtual void SetProperties(IEnumerable<IPropertyMetadata> propertyMetadatas)
        {
            if (propertyMetadatas == null)
                throw new ArgumentNullException("propertyMetadatas");
            Properties = propertyMetadatas;

            ViewProperties = new ReadOnlyCollection<IPropertyMetadata>(propertyMetadatas.Where(t => !t.IsHiddenOnView && t.CanGet).ToArray());
            CreateProperties = new ReadOnlyCollection<IPropertyMetadata>(propertyMetadatas.Where(t => !t.IsHiddenOnCreate && t.CanSet).ToArray());
            EditProperties = new ReadOnlyCollection<IPropertyMetadata>(propertyMetadatas.Where(t => !t.IsHiddenOnEdit && t.CanSet).ToArray());
            SearchProperties = new ReadOnlyCollection<IPropertyMetadata>(propertyMetadatas.Where(t => t.Searchable).ToArray());
            DetailProperties = new ReadOnlyCollection<IPropertyMetadata>(propertyMetadatas.Where(t => !t.IsHiddenOnDetail && t.CanGet).ToArray());

            Dictionary<string, IPropertyMetadata> cache = Properties.ToDictionary(t => t.ClrName, t => t);
            PropertyCache = new ReadOnlyDictionary<string, IPropertyMetadata>(cache);
        }

        /// <summary>
        /// Set the metadata of authentication.
        /// </summary>
        /// <param name="authentication"></param>
        protected virtual void SetAuthentication(EntityAuthenticationAttribute authentication)
        {
            if (authentication == null)
                throw new ArgumentNullException("authentication");
            AllowAnonymous = authentication.AllowAnonymous;
            AddRoles = new ReadOnlyCollection<object>(authentication.AddRolesRequired);
            EditRoles = new ReadOnlyCollection<object>(authentication.EditRolesRequired);
            ViewRoles = new ReadOnlyCollection<object>(authentication.ViewRolesRequired);
            RemoveRoles = new ReadOnlyCollection<object>(authentication.RemoveRolesRequired);
            AuthenticationRequiredMode = authentication.Mode;
        }

        /// <summary>
        /// Set the metadata automatic.
        /// </summary>
        protected virtual void SetMetadata()
        {

            DisplayNameAttribute display = Type.GetCustomAttribute<DisplayNameAttribute>();
            if (display != null)
                SetDisplay(display);
            else
                Name = Type.Name;

            DisplayColumnAttribute displayColumn = Type.GetCustomAttribute<DisplayColumnAttribute>();
            if (displayColumn != null)
                SetDisplayColumn(displayColumn);
            else
                DisplayProperty = GetProperty("Index");


            ParentAttribute parent = Type.GetCustomAttribute<ParentAttribute>();
            if (parent != null)
                SetParent(parent);

            EntityAuthenticationAttribute authenticate = Type.GetCustomAttribute<EntityAuthenticationAttribute>();
            if (authenticate == null)
            {
                AllowAnonymous = true;
                AddRoles = new string[0];
                EditRoles = new string[0];
                ViewRoles = new string[0];
                RemoveRoles = new string[0];
            }
            else
                SetAuthentication(authenticate);
        }
    }
}
