using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        private readonly IReadOnlyList<Type> _handlers;
        private readonly IServiceProvider _serviceProvider;

        public AuthenticationProvider(IServiceProvider serviceProvider, IOptions<AuthenticationProviderOptions> options)
        {
            _serviceProvider = serviceProvider;
            _handlers = options.Value.Handlers;
        }

        public async Task<ClaimsPrincipal> GetUserAsync()
        {
            for (int i = 0; i < _handlers.Count; i++)
            {
                IAuthenticationHandler handler = (IAuthenticationHandler)ActivatorUtilities.CreateInstance(_serviceProvider, _handlers[i]);
                var result = await handler.AuthenticateAsync();
                if (result.IsSuccess)
                {
                    return result.Principal!;
                }
            }
            ClaimsIdentity identity = new ClaimsIdentity("Anonymous");
            return new ClaimsPrincipal(identity);
        }
    }
}
