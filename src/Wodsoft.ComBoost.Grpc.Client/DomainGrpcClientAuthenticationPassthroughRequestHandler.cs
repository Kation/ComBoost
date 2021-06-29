using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public class DomainGrpcClientAuthenticationPassthroughRequestHandler : IDomainRpcClientRequestHandler
    {
        public void Handle(IDomainRpcRequest request, IDomainContext context)
        {
            var authenticationProvider = context.GetRequiredService<IAuthenticationProvider>();
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            authenticationProvider.User.WriteTo(writer);
            request.Headers["Authentication"] = stream.ToArray();
        }
    }
}
