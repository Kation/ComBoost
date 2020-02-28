using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Data
{
    public class EntityDomainAuthorizeOption
    {
        public static readonly EntityDomainAuthorizeOption Anonymous = new EntityAnonymousAuthorizeOption();
        public static readonly EntityDomainAuthorizeOption Empty = new EntityDomainAuthorizeOption();
        public static readonly EntityDomainAuthorizeOption View = new EntityDomainActionAuthorizeOption(metadata => metadata.ViewRoles, metadata => metadata.ViewProperties, metadata => metadata.ViewRoles);
        public static readonly EntityDomainAuthorizeOption Create = new EntityDomainActionAuthorizeOption(metadata => metadata.AddRoles, metadata => metadata.CreateProperties, metadata => metadata.AddRoles);
        public static readonly EntityDomainAuthorizeOption Edit = new EntityDomainActionAuthorizeOption(metadata => metadata.EditRoles, metadata => metadata.EditProperties, metadata => metadata.EditRoles);
        public static readonly EntityDomainAuthorizeOption Remove = new EntityDomainActionAuthorizeOption(metadata => metadata.RemoveRoles, metadata => new IPropertyMetadata[0], metadata => new object[0]);
        public static readonly EntityDomainAuthorizeOption Detail = new EntityDomainActionAuthorizeOption(metadata => metadata.ViewRoles, metadata => metadata.DetailProperties, metadata => metadata.ViewRoles);

        public virtual void Validate(IEntityMetadata metadata, IAuthentication authentication)
        {
            if (!metadata.AllowAnonymous && !authentication.Identity.IsAuthenticated)
                throw new DomainServiceException(new UnauthorizedAccessException("未登录。"));
        }

        public virtual IEnumerable<IPropertyMetadata> GetProperties(IEntityMetadata metadata, IAuthentication authentication)
        {
            return metadata.Properties;
        }
    }

    public class EntityDomainActionAuthorizeOption : EntityDomainAuthorizeOption
    {
        public EntityDomainActionAuthorizeOption(Func<IEntityMetadata, IEnumerable<object>> entityRolesSelector,
            Func<IEntityMetadata, IEnumerable<IPropertyMetadata>> propertiesSelector,
            Func<IPropertyMetadata, IEnumerable<object>> propertyRolesSelector)
        {
            if (entityRolesSelector == null)
                throw new ArgumentNullException(nameof(entityRolesSelector));
            if (propertiesSelector == null)
                throw new ArgumentNullException(nameof(propertiesSelector));
            if (propertyRolesSelector == null)
                throw new ArgumentNullException(nameof(propertyRolesSelector));
            EntityRolesSelector = entityRolesSelector;
            PropertiesSelector = propertiesSelector;
            PropertyRolesSelector = propertyRolesSelector;
        }

        public Func<IEntityMetadata, IEnumerable<object>> EntityRolesSelector { get; private set; }

        public Func<IEntityMetadata, IEnumerable<IPropertyMetadata>> PropertiesSelector { get; private set; }

        public Func<IPropertyMetadata, IEnumerable<object>> PropertyRolesSelector { get; private set; }

        public override void Validate(IEntityMetadata metadata, IAuthentication authentication)
        {
            base.Validate(metadata, authentication);
            var roles = EntityRolesSelector(metadata).ToArray();
            if (roles.Length == 0)
                return;
            if (metadata.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
            {
                if (roles.All(t => !authentication.IsInRole(t)))
                    throw new DomainServiceException(new UnauthorizedAccessException("权限不足。"));
            }
            else
            {
                if (roles.Any(t => !authentication.IsInRole(t)))
                    throw new DomainServiceException(new UnauthorizedAccessException("权限不足。"));
            }
        }

        public override IEnumerable<IPropertyMetadata> GetProperties(IEntityMetadata metadata, IAuthentication authentication)
        {
            return PropertiesSelector(metadata).Where(t =>
            {
                if (!t.AllowAnonymous && !authentication.Identity.IsAuthenticated)
                    return false;
                var roles = PropertyRolesSelector(t).ToArray();
                if (roles.Length == 0)
                    return true;
                if (t.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                    return roles.All(r => authentication.IsInRole(r));
                else
                    return roles.Any(r => authentication.IsInRole(r));
            });
        }
    }

    public class EntityAnonymousAuthorizeOption : EntityDomainAuthorizeOption
    {
        public override void Validate(IEntityMetadata metadata, IAuthentication authentication) { }
    }
}
