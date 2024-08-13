using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.CAP
{
    public class DomainCAPEventPublisher<T> : IDomainServiceEventHandler<T>
        where T : DomainServiceEventArgs
    {
        private static readonly string _Name = GetTypeName(typeof(T));

        private static string GetTypeName(Type type)
        {
            var name = type.Namespace + "." + type.Name;
            if (type.IsGenericType)
            {
                name += "<" + string.Join(",", type.GetGenericArguments().Select(t => GetTypeName(t))) + ">";
            }
            return name;
        }

        public Task Handle(IDomainExecutionContext context, T args)
        {
            var publisher = context.DomainContext.GetRequiredService<ICapPublisher>();
            return publisher.PublishAsync<T>(_Name, args);
        }
    }
}
