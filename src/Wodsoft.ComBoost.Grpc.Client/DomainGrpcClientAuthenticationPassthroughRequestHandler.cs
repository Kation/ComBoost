using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Grpc.Client
{
    public class DomainGrpcClientAuthenticationPassthroughRequestHandler : IDomainRpcClientRequestHandler
    {
        public async Task HandleAsync(IDomainRpcRequest request, IDomainContext context)
        {
            var authenticationProvider = context.GetRequiredService<IAuthenticationProvider>();
            var user = await authenticationProvider.GetUserAsync();
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            user.WriteTo(writer);
            request.Headers["Authentication"] = stream.ToArray();
        }
    }
}
