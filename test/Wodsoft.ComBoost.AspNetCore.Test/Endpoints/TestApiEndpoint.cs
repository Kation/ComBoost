using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.AspNetCore.Test.Services;

namespace Wodsoft.ComBoost.AspNetCore.Test.Endpoints
{
    [DomainEndpoint]
    public partial class TestApiEndpoint : ApiEndpoint<ITestDomainService>
    {
    }
}
