using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Text;

namespace Wodsoft.ComBoost.Grpc.AspNetCore
{
    public class DomainGrpcServiceOptions
    {
        private Func<HttpContext, IDomainRpcRequest, ClaimsPrincipal> _authenticationHandler = (context, request) => context.User;
        public Func<HttpContext, IDomainRpcRequest, ClaimsPrincipal> AuthenticationHandler
        {
            get => _authenticationHandler; set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _authenticationHandler = value;
            }
        }
    }
}
