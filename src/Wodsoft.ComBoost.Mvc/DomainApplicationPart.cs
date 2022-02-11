using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
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
                if (serviceType == null)
                    return null;
                return ((Type)typeof(DomainControllerBuilder<,>).MakeGenericType(t, serviceType).GetProperty("ControllerType")!.GetValue(null)!).GetTypeInfo();
            }).Where(t => t != null).Cast<TypeInfo>().ToArray();
        }

        public IEnumerable<TypeInfo> Types { get; }

        public override string Name { get; }
    }
}
