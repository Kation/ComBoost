using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public class DomainGrpcContext : DomainRpcContext
    {
        public DomainGrpcContext(DomainGrpcRequest request, ServerCallContext callContext) : base(request, callContext.GetHttpContext().RequestServices, callContext.GetHttpContext().RequestAborted)
        {
            if (callContext == null)
                throw new ArgumentNullException(nameof(callContext));
            CallContext = callContext;
            HttpContext = callContext.GetHttpContext();
        }

        public ServerCallContext CallContext { get; }

        public HttpContext HttpContext { get; }
    }
}
