using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost.Mvc
{
    public class DomainApplicationPart : ApplicationPart, IApplicationPartTypeProvider
    {
        public DomainApplicationPart(Assembly assembly)
        {
            Name = assembly.GetName().Name + " ComBoost Application";
            Types = assembly.GetTypes().Where(t => typeof(IDomainController).IsAssignableFrom(t)).Select(t =>
            {
                var serviceType = DomainController.GetDomainServiceType(t);
                return ((Type)typeof(DomainControllerBuilder<,>).MakeGenericType(t, serviceType).GetProperty("ControllerType").GetGetMethod().Invoke(null, null)).GetTypeInfo();
            }).ToArray();
        }

        public IEnumerable<TypeInfo> Types { get; }

        public override string Name { get; }
    }
}
