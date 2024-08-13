using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public class MockAuthenticationSettings
    {
        public ClaimsPrincipal User { get; set; } = new ClaimsPrincipal();
    }
}
