using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mock
{
    public class MockAuthenticationHandler : IAuthenticationHandler
    {
        private readonly MockAuthenticationSettings _settings;

        public MockAuthenticationHandler(MockAuthenticationSettings settings)
        {
            _settings = settings;
        }

        public Task<AuthenticationResult> AuthenticateAsync()
        {
            if (_settings.User.Identities.Any())
                return Task.FromResult(AuthenticationResult.Success(_settings.User));
            return Task.FromResult(AuthenticationResult.Fail());
        }
    }
}
