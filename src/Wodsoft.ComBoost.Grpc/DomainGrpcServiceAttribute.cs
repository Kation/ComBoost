using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class DomainGrpcServiceAttribute : Attribute
    {
        public DomainGrpcServiceAttribute() { }

        public DomainGrpcServiceAttribute(string serviceName)
        {
            ServiceName = serviceName;
        }

        public DomainGrpcServiceAttribute(Type endPointProvider)
        {
            if (!typeof(IDomainGrpcEndPointProvider).IsAssignableFrom(endPointProvider))
                throw new ArgumentException("EndPoint provider must implement IDomainGrpcEndPointProvider.");
            if (endPointProvider.GetConstructor(Array.Empty<Type>()) == null)
                throw new ArgumentException("EndPoint provider must have public empty constructor.");
            EndPointProvider = endPointProvider;
        }

        public string? ServiceName { get; }

        public Type? EndPointProvider { get; }
    }
}
