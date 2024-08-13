using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Grpc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DomainGrpcMethodAttribute : Attribute
    {
        public DomainGrpcMethodAttribute() { }

        public DomainGrpcMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        public DomainGrpcMethodAttribute(string serviceName, string methodName)
        {
            ServiceName = serviceName;
            MethodName = methodName;
        }

        public DomainGrpcMethodAttribute(Type endPointProvider)
        {
            if (!typeof(IDomainGrpcEndPointProvider).IsAssignableFrom(endPointProvider))
                throw new ArgumentException("EndPoint provider must implement IDomainGrpcEndPointProvider.");
            if (endPointProvider.GetConstructor(Array.Empty<Type>()) == null)
                throw new ArgumentException("EndPoint provider must have public empty constructor.");
            EndPointProvider = endPointProvider;
        }

        public string? ServiceName { get; }

        public string? MethodName { get; }

        public Type? EndPointProvider { get; }
    }
}
