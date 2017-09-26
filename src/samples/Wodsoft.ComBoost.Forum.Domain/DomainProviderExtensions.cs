using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;

namespace Wodsoft.ComBoost.Forum.Domain
{
    public static class DomainProviderExtensions
    {
        public static void AddForumExtensions(this IDomainServiceProvider provider)
        {
            provider.AddGenericDefinitionExtension(typeof(EntityDomainService<>), typeof(ThreadDomainExtension<>));
            provider.AddGenericDefinitionExtension(typeof(EntityDomainService<>), typeof(PostDomainExtension<>));
        }
    }
}
