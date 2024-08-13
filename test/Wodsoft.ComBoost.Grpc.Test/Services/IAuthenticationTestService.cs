﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Grpc.Test.Services
{
    public interface IAuthenticationTestService : IDomainTemplate
    {
        [DomainGrpcMethod("Authentication", "GetRoles")]
        Task<string[]> GetRoles();
    }
}
