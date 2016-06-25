//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace Wodsoft.ComBoost.Security
//{
//    public class ComBoostIdentity : ClaimsIdentity
//    {
//        public ComBoostIdentity()
//        {

//        }

//        public override bool HasClaim(Predicate<Claim> match)
//        {
//            return Claims.Any(t =>
//             {
//                 if (!
//                 match(t))
//                     return false;
//                 if (t is ComBoostClaim && ((ComBoostClaim)t).ExpiredDate < DateTime.Now)
//                     return false;
//                 return true;
//             });
//        }

//        public override bool HasClaim(string type, string value)
//        {
//            return Claims.Any(t =>
//            {
//                if (t.Type != type || t.Value != value)
//                    return false;
//                if (t is ComBoostClaim && ((ComBoostClaim)t).ExpiredDate < DateTime.Now)
//                    return false;
//                return true;
//            });
//        }
//    }
//}
