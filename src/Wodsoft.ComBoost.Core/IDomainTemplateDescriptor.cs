using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainTemplateDescriptor<T>
        where T: class, IDomainTemplate
    {
        T GetTemplate(IDomainContext context);
    }
}
