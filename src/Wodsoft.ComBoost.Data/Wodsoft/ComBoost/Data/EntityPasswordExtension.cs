using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Data
{
    public class EntityPasswordExtension<T> : DomainExtension
        where T : class, IEntity, IHavePassword, new()
    {
        private EntityDomainService<T> Service;

        public override void OnInitialize(IServiceProvider serviceProvider, IDomainService domainService)
        {
            Service = (EntityDomainService<T>)domainService;
            Service.EntityPropertyUpdate += Service_EntityPropertyUpdate;
        }

        private void Service_EntityPropertyUpdate(IDomainExecutionContext context, EntityPropertyUpdateEventArgs<T> e)
        {
            if (e.Property.Type == System.ComponentModel.DataAnnotations.CustomDataType.Password)
            {
                if ((string)e.Value == "")
                    if (e.Entity.IsNewCreated)
                        return;
                    else
                        throw new ArgumentNullException("“" + e.Property.Name + "”不能为空。");
                e.Entity.SetPassword((string)e.Value);
                e.IsHandled = true;
            }
        }
    }
}
