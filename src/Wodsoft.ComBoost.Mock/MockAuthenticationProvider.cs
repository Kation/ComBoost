//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Wodsoft.ComBoost.Security;

//namespace Wodsoft.ComBoost.Mock
//{
//    public class MockAuthenticationProvider : IAuthenticationProvider
//    {
//        private MockPrincipal _CurrentPrincipal;

//        public MockAuthenticationProvider()
//        {
//            _CurrentPrincipal = new MockPrincipal(securityProvider);
//        }

//        public ClaimsPrincipal User => _CurrentPrincipal;

//        public bool IsInRole(string role)
//        {

//        }
//    }
//}
