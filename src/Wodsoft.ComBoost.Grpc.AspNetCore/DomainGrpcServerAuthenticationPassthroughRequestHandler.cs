using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public class DomainGrpcServerAuthenticationPassthroughRequestHandler : IDomainRpcServerRequestHandler
    {
        public void Handle(IDomainRpcRequest request, DomainRpcContext context)
        {
            if (request.Headers.TryGetValue("Authentication", out var data))
            {
                MemoryStream stream = new MemoryStream(data);
                BinaryReader reader = new BinaryReader(stream);
                ClaimsPrincipal principal = new ClaimsPrincipal(reader);
                DomainGrpcAuthenticationProvider authenticationProvider = new DomainGrpcAuthenticationProvider(principal);
                context.SetService<IAuthenticationProvider>(authenticationProvider);
            }
        }
    }
}
