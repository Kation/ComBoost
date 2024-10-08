﻿using Grpc.AspNetCore.Server.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.AspNetCore;
using Wodsoft.ComBoost.Grpc.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainGrpcServerDependencyInjectionExtensions
    {
        public static IComBoostGrpcBuilder AddGrpcServices(this IComBoostAspNetCoreBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IServiceMethodProvider<DomainGrpcDiscoveryService>), typeof(DomainGrpcServiceMethodProvider)));
            return new ComBoostGrpcBuilder(builder.Services, builder);
        }

        public static IComBoostGrpcBuilder UseAuthentication(this IComBoostGrpcBuilder builder, Func<HttpContext, IDomainRpcRequest, ClaimsPrincipal> handler)
        {
            builder.Services.PostConfigure<DomainGrpcServiceOptions>(options => options.AuthenticationHandler = handler);
            return builder;
        }

        public static IComBoostGrpcBuilder AddAuthenticationPassthrough(this IComBoostGrpcBuilder builder)
        {
            return UseAuthentication(builder, (context, request) =>
            {
                if (request.Headers.TryGetValue("Authentication", out var data))
                {
                    MemoryStream stream = new MemoryStream(data);
                    BinaryReader reader = new BinaryReader(stream);
                    ClaimsPrincipal principal = new ClaimsPrincipal(reader);
                    return principal;
                }
                return context.User;
            });
        }

        private readonly static MethodInfo _AddTemplateMethod = typeof(IComBoostGrpcBuilder).GetMethod("AddTemplate")!;
        public static IComBoostGrpcBuilder AddTemplateInAssembly(this IComBoostGrpcBuilder builder, string serviceName, Assembly assembly)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (serviceName == null)
                throw new ArgumentNullException(nameof(serviceName));
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsInterface && !type.IsGenericTypeDefinition && type.GetInterfaces().Any(t => t == typeof(IDomainTemplate)))
                {
                    var attr = type.GetCustomAttribute<DomainDistributedServiceAttribute>();
                    if (attr != null && attr.ServiceName == serviceName)
                        _AddTemplateMethod.MakeGenericMethod(type).Invoke(builder, Array.Empty<object>());
                }
            }
            return builder;
        }

        public static IComBoostGrpcBuilder AddTemplateInAssembly(this IComBoostGrpcBuilder builder, Assembly assembly)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsInterface && !type.IsGenericTypeDefinition && type.GetInterfaces().Any(t => t == typeof(IDomainTemplate)))
                {
                    _AddTemplateMethod.MakeGenericMethod(type).Invoke(builder, Array.Empty<object>());
                }
            }
            return builder;
        }
    }
}
