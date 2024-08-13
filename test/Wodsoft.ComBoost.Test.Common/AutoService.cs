using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    [AutoTemplate(Group = "internal", TemplateName = "IAutoInternalService")]
    public partial class AutoService : DomainService
    {
        public Task Test1(string text) { return Task.CompletedTask; }

        [AutoTemplateMethod(IsExcluded = true)]
        public Task Test2(string text) { return Task.CompletedTask; }
    }
}
