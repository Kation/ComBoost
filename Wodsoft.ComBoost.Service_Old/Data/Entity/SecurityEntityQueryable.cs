using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace System.Data.Entity
{
    public class SecurityEntityQueryable<TEntity> : IEntityQueryable<TEntity> where TEntity : EntityBase, new()
    {
        private string[] addable, editable, removeable;
        private Dictionary<string, string[]> editableProperties, visableProperties;
        private string[] properties;

        public SecurityEntityQueryable(DbContext dbContext)
            : base(dbContext)
        {
            var eaa = (EntityAuthenticationAttribute)typeof(TEntity).GetCustomAttributes(typeof(EntityAuthenticationAttribute), true).LastOrDefault();
            if (eaa == null)
            {
                addable = new string[0];
                editable = new string[0];
                removeable = new string[0];
            }
            else
            {
                if (eaa.AddRolesRequired == null && eaa.AllowAnonymous)
                    addable = new string[0];
                else
                    addable = eaa.AddRolesRequired;
                if (eaa.EditRolesRequired == null && eaa.AllowAnonymous)
                    editable = new string[0];
                else
                    editable = eaa.EditRolesRequired;
                if (eaa.RemoveRolesRequired == null && eaa.AllowAnonymous)
                    removeable = new string[0];
                else
                    removeable = eaa.RemoveRolesRequired;
            }

            editableProperties = new Dictionary<string, string[]>();
            visableProperties = new Dictionary<string, string[]>();

            foreach (var property in base.EditableProperties())
            {                
                var paa = (PropertyAuthenticationAttribute)typeof(TEntity).GetProperty(property).GetCustomAttributes(typeof(PropertyAuthenticationAttribute), true).LastOrDefault();
                if (paa == null)
                    editableProperties.Add(property, new string[0]);
                else
                    if (paa.EditRolesRequired == null && paa.AllowAnonymous)
                        editableProperties.Add(property, new string[0]);
                    else
                        editableProperties.Add(property, paa.EditRolesRequired);
            }

            properties = typeof(TEntity).GetProperties().Select(t => t.Name).ToArray();

            foreach (var property in base.VisableProperties())
            {
                var paa = (PropertyAuthenticationAttribute)typeof(TEntity).GetProperty(property).GetCustomAttributes(typeof(PropertyAuthenticationAttribute), true).LastOrDefault();
                if (paa == null)
                    visableProperties.Add(property, new string[0]);
                else
                    if (paa.ViewRolesRequired == null && paa.AllowAnonymous)
                        visableProperties.Add(property, new string[0]);
                    else
                        visableProperties.Add(property, paa.ViewRolesRequired);
            }
        }
        
        public override bool Add(TEntity entity)
        {
            if (!Addable())
                return false;
            return base.Add(entity);
        }

        public override bool Edit(TEntity entity)
        {
            if (!Editable())
                return false;
            var entry = DbContext.Entry<TEntity>(entity);
            foreach (var property in properties.Except(EditableProperties()))
                entry.Property(property).IsModified = false;
            return base.Edit(entity);
        }

        public override bool Remove(Guid entityID)
        {
            if (!Removeable())
                return false;
            return base.Remove(entityID);
        }

        public override bool Addable()
        {
            if (addable==null)
                return false;
            return RoleManager.HasRoles(addable);
        }

        public override bool Editable()
        {
            if (editable == null)
                return false;
            return RoleManager.HasRoles(editable);
        }

        public override bool Removeable()
        {
            if (removeable == null)
                return false;
            return RoleManager.HasRoles(removeable);
        }

        public override string[] EditableProperties()
        {
            return editableProperties.Except(editableProperties.Where(t => !RoleManager.HasRoles(t.Value))).Select(t => t.Key).ToArray();
        }

        public override string[] VisableProperties()
        {
            return visableProperties.Except(visableProperties.Where(t => !RoleManager.HasRoles(t.Value))).Select(t => t.Key).ToArray();
        }

        public override TEntity GetEntity(Guid entityID)
        {
            TEntity entity = base.GetEntity(entityID);
            Type type = typeof(TEntity);
            foreach (var property in properties.Except(VisableProperties()))
            {
                var propertyInfo = type.GetProperty(property);
                if (propertyInfo.GetCustomAttributes(typeof(HideAttribute), true).Count() > 0)
                    continue;
                if (propertyInfo.PropertyType.IsValueType)
                    propertyInfo.SetValue(entity, Activator.CreateInstance(propertyInfo.PropertyType), null);
                else
                    propertyInfo.SetValue(entity, null, null);
            }
            return entity;
        }

        public override IQueryable<TEntity> Query()
        {
            var result = base.Query();
            Type type = typeof(TEntity);
            foreach (var property in properties.Except(EditableProperties()))
            {
                var propertyInfo = type.GetProperty(property);
                if (propertyInfo.Name == "BaseIndex")
                    continue;
                if (propertyInfo.PropertyType.IsValueType)
                    foreach (var item in result)
                        propertyInfo.SetValue(item, Activator.CreateInstance(propertyInfo.PropertyType), null);
                else
                    foreach (var item in result)
                        propertyInfo.SetValue(item, null, null);
            }
            return result;
        }
    }
}
