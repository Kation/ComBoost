using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Mvc.Test.Services;

namespace Wodsoft.ComBoost.Mvc.Test.Controllers
{
    public partial class TestController : ControllerBase, IDomainAction<ITestDomainService>
    {
    }
}
